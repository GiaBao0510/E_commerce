using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.Core.Exceptions;
using E_commerce.SQL.Queries;
using Microsoft.AspNetCore.JsonPatch;

namespace E_commerce.Infrastructure.repositories
{
    public class MessageRepository: BaseRepository<_Message>, IMessageRepository
    {
                /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public MessageRepository(
            ILogger logger,
            IUnitOfWork unitOfWork
        ): base(unitOfWork, logger)
        { }

        /// <summary>
        /// Kiểm tra tính hợp lệ của Department
        /// </summary
        public void ValidateMessage(_Message message){
            if(message == null || string.IsNullOrWhiteSpace(message.text))
                throw new ValidationException("Thông tin tin nhắn không được thiếu xót");
        }

        /// <summary>
        /// Lấy tất cả thông tin
        /// </summary>
        public override async Task<IReadOnlyList<_Message>> GetAllAsync(){
            try{
                var messages = await Connection.QueryAsync<_Message>(
                    MessageQueries.GetAll,
                    transaction: Transaction
                );
                return messages.ToList();
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách tin nhắn", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách tin nhắn");
            }
        }
        
        /// <summary>
        /// Lấy tất cả thông tin theo ID
        /// </summary>
        public override async Task<_Message> GetByIdAsync(string id){
            try{
                
                if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");
                
                var message = await Connection.QueryFirstOrDefaultAsync<_Message>(
                    MessageQueries.FindByID,
                    new { message_id = id },
                    transaction: Transaction
                );

                if(message == null)
                    throw new ResourceNotFoundException($"Không tìm thấy ID tin nhắn: {id}");
                return message;
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách theo ID tin nhắn", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách theo ID tin nhắn");
            }
        }
        
        /// <summary>
        /// Thêm thông tin
        /// </summary>
        public override async Task<string> AddAsync(_Message entity){
            try{

                //Kiểm tra thông tin cuộc trò chuyên
                if(await _unitOfWork.conversations.GetByIdAsync(entity.conversation_id) == null)
                    throw new ResourceNotFoundException($"Không tìm thấy ID cuộc hội thoại: {entity.conversation_id}");
                
                //Kiểm tra thông tin người gửi
                if(await _unitOfWork.users.GetByIdAsync(entity.from_number) == null)
                    throw new ResourceNotFoundException($"Không tìm thấy ID người gửi: {entity.from_number}");

                //Thêm
                var message = await Connection.ExecuteAsync(
                    MessageQueries.cretae,
                    entity,
                    transaction: Transaction
                );
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi thêm thông tin tin nhắn", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi thêm thông tin tin nhắn");
            }
        }

        public override async Task<string> UpdateAsync(_Message entity){
            try{

                ValidateMessage(entity);

                var message = await Connection.ExecuteAsync(
                    MessageQueries.UpdateByID,
                    entity,
                    transaction: Transaction
                );

                if(message == 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID tin nhắn: {entity.mess_id}");
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi cập nhật thông tin tin nhắn", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi cập nhật thông tin tin nhắn");
            }
        }

        public override async Task<string> DeleteAsync(string id){
            try{
                
                if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");

                var message = await Connection.ExecuteAsync(
                    MessageQueries.DeleteByID,
                    new { mess_id = id },
                    transaction: Transaction
                );

                if(message == 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID tin nhắn: {id}");
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi xóa thông tin tin nhắn", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xóa thông tin tin nhắn");
            }
        }
        public override async Task<string> PatchAsync(string id, JsonPatchDocument<_Message> patchDoc){
            
            if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");

            if(patchDoc == null)
                throw new ValidationException("Thông tin cập nhật không được bỏ trống");    
            
            var message = await GetByIdAsync(id);
            if(message == null)
                throw new ResourceNotFoundException($"Không tìm thấy ID tin nhắn: {id}");

            //Áp dụng các thay đổi
            patchDoc.ApplyTo(message);

            try{
                var result = await Connection.ExecuteAsync(
                    MessageQueries.UpdatePatchByID,
                    message,
                    transaction: Transaction
                );
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi cập thông tin tin nhắn", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xóa thông tin tin nhắn");
            }
        }

        /// <summary>
        /// Lấy danh sách các tin nhắn dựa trên cuộc hội thoại (one to one)
        /// </summary>
        public async Task<IReadOnlyList<_Message>> ListOfMessagesByConversationID(string conversation_id){
            try{
                
                //Kiểm tra đầu vào
                if(await _unitOfWork.conversations.GetByIdAsync(conversation_id) == null)
                    throw new ResourceNotFoundException($"Không tìm thấy ID cuộc hội thoại: {conversation_id}");

                var result = await Connection.QueryAsync<_Message>(
                    MessageQueries.ListOfMessagesByConversationID,
                    new { conversation_id = conversation_id },
                    transaction: Transaction
                );

                return result.ToList();
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi Lấy danh sách các tin nhắn dựa trên cuộc hội thoại (one to one)", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi Lấy danh sách các tin nhắn dựa trên cuộc hội thoại (one to one)");
            }
        }

        /// <summary>
        /// Lấy danh sách các tin nhắn dựa trên Group Chat ID
        /// </summary>
        public async Task<IReadOnlyList<_Message>> ListOfMessagesByGroupChatID(string group_id){
            try{
                
                //Kiểm tra đầu vào
                if(await _unitOfWork.groupChat.GetByIdAsync(group_id) == null)
                    throw new ResourceNotFoundException($"Không tìm thấy ID nhóm trò chuyện: {group_id}");

                var result = await Connection.QueryAsync<_Message>(
                    MessageQueries.listOfMessagesByGroupChatID,
                    new { group_id = group_id },
                    transaction: Transaction
                );

                return result.ToList();
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi Lấy danh sách các tin nhắn dựa trên Group Chat ID", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi Lấy danh sách các tin nhắn dựa trên Group Chat ID");
            }
        }
    }
}