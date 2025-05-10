using Dapper;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.Core.Exceptions;
using E_commerce.Infrastructure.Constants;
using E_commerce.SQL.Queries;
using Microsoft.AspNetCore.JsonPatch;
using MySql.Data.MySqlClient;

namespace E_commerce.Infrastructure.repositories
{
    public class GroupChatRepository: BaseRepository<_GroupChat>, IGroupChatRepository
    {
        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public GroupChatRepository(
            ILogger logger,
            IUnitOfWork unitOfWork
        ): base(unitOfWork, logger)
        { }

        /// <summary>
        /// Kiểm tra tính hợp lệ của Department
        /// </summary
        public async Task ValidateGroupChat(_GroupChat groupchat){
            
            if(groupchat == null || string.IsNullOrWhiteSpace(groupchat.group_name))
                throw new ValidationException("Thông tin nhóm chat không được thiếu xót");

            //Kiểm tra mã người dùng có tồn tại không
            if(await _unitOfWork.users.GetByIdAsync(groupchat.user_id) == null)
                throw new ResourceNotFoundException($"Không tìm thấy ID người dùng: {groupchat.user_id}");

            //Kiểm tra xem conversation ID có tồn tại không
            if(await _unitOfWork.conversations.GetByIdAsync(groupchat.conversation_id) == null)
                throw new ResourceNotFoundException($"Không tìm thấy ID cuộc trò chuyện: {groupchat.conversation_id}");
        }

        /// <summary>
        /// Lấy tất cả thông tin
        /// </summary>
        public override async Task<IReadOnlyList<_GroupChat>> GetAllAsync(){
            try{
                var groupchats = await Connection.QueryAsync<_GroupChat>(
                    GroupChatQueries.GetAll,
                    transaction: Transaction
                );
                return groupchats.ToList();
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách nhóm chat", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách nhóm chat");
            }
        }
        
        /// <summary>
        /// Lấy tất cả thông tin theo ID
        /// </summary>
        public override async Task<_GroupChat> GetByIdAsync(string id){
            try{
                
                if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");
                
                var groupchat = await Connection.QueryFirstOrDefaultAsync<_GroupChat>(
                    GroupChatQueries.FindByID,
                    new { group_id = id },
                    transaction: Transaction
                );

                if(groupchat == null)
                    throw new ResourceNotFoundException($"Không tìm thấy ID nhóm chat: {id}");
                return groupchat;
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách theo ID nhóm chat", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách theo ID nhóm chat");
            }
        }
        
        /// <summary>
        /// Thêm thông tin
        /// </summary>
        public override async Task<string> AddAsync(_GroupChat entity){
            try{
                
                await ValidateGroupChat(entity);

                var groupchat = await Connection.ExecuteAsync(
                    GroupChatQueries.Add,
                    entity,
                    transaction: Transaction
                );
                return "SUCCESS";
            }
            catch(MySqlException ex){
            
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR)
                    throw new ResourceConflictException("ID GroupChat bị trùng lặp");

                _logger.Error($"Database error when updating GroupChat \n Error number:{ex.Number} \nMessage:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex,"Lỗi khi thêm GroupChat vào cơ sở dữ liệu");
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi thêm thông tin nhóm chat", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi thêm thông tin nhóm chat");
            }
        }

        public override async Task<string> UpdateAsync(_GroupChat entity){
            try{

                await ValidateGroupChat(entity);

                var groupchat = await Connection.ExecuteAsync(
                    GroupChatQueries.UpdateByID,
                    entity,
                    transaction: Transaction
                );

                if(groupchat == 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID nhóm chat: {entity.group_id}");
                return "SUCCESS";
            }
            catch(MySqlException ex){
            
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR)
                    throw new ResourceConflictException("ID GroupChat bị trùng lặp");

                _logger.Error($"Database error when updating GroupChat \n Error number:{ex.Number} \nMessage:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex,"Lỗi khi cập nhật GroupChat vào cơ sở dữ liệu");
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi cập nhật thông tin nhóm chat", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi cập nhật thông tin nhóm chat");
            }
        }

        public override async Task<string> DeleteAsync(string id){
            try{
                
                if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");

                var groupchat = await Connection.ExecuteAsync(
                    GroupChatQueries.DeleteByID,
                    new { group_id = id },
                    transaction: Transaction
                );

                if(groupchat == 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID nhóm chat: {id}");
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi xóa thông tin nhóm chat", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xóa thông tin nhóm chat");
            }
        }
        public override async Task<string> PatchAsync(string id, JsonPatchDocument<_GroupChat> patchDoc){
            
            if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");

            if(patchDoc == null)
                throw new ValidationException("Thông tin cập nhật không được bỏ trống");    
            
            var groupchat = await GetByIdAsync(id);
            if(groupchat == null)
                throw new ResourceNotFoundException($"Không tìm thấy ID nhóm chat: {id}");

            //Áp dụng các thay đổi
            patchDoc.ApplyTo(groupchat);

            try{
                var result = await Connection.ExecuteAsync(
                    GroupChatQueries.PatchByID,
                    groupchat,
                    transaction: Transaction
                );
                return "SUCCESS";
            }
            catch(MySqlException ex){
            
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR)
                    throw new ResourceConflictException("ID GroupChat bị trùng lặp");

                _logger.Error($"Database error when updating GroupChat \n Error number:{ex.Number} \nMessage:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex,"Lỗi khi cập nhật GroupChat vào cơ sở dữ liệu");
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi cập thông tin nhóm chat", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xóa thông tin nhóm chat");
            }
        }

        /// <summary>
        /// Danh sách các cuộc hội thoại của người dùng dựa trên UserID
        /// </summary>
        public async Task<IReadOnlyList<_Conversation>> GetConversationByUserID(string uid){
            try{
                //Kiểm tra mã người dùng có tồn tại không
                if(await _unitOfWork.users.GetByIdAsync(uid) == null)
                    throw new ResourceNotFoundException($"Không tìm thấy ID người dùng: {uid}");

                var groupchats = await Connection.QueryAsync<_Conversation>(
                    GroupChatQueries.GetConversationByUserID,
                    new{ user_id = uid },
                    transaction: Transaction
                );
                return groupchats.ToList();
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách danh sách các cuộc trò chuyện của người dùng", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách các cuộc trò chuyện của người dùng");
            }
        }

        /// <summary>
        /// Danh sách các người dùng trong nhóm dựa trên ConversationID
        /// </summary>
        public async Task<IReadOnlyList<_User>> GetUserByConversationID(string conversation_id){
            try{

                //Kiểm tra xem conversation ID có tồn tại không
                if(await _unitOfWork.conversations.GetByIdAsync(conversation_id) == null)
                    throw new ResourceNotFoundException($"Không tìm thấy ID cuộc trò chuyện: {conversation_id}");
                    
                var groupchats = await Connection.QueryAsync<_User>(
                    GroupChatQueries.GetUserByConversationID,
                    new { conversation_id = conversation_id },
                    transaction: Transaction
                );
                return groupchats.ToList();
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách danh sách người dùng trong cuộc trò chuyện", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách người dùng trong cuộc trò chuyện");
            }
        }
    }
}