
using Dapper;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.Core.Exceptions;
using E_commerce.SQL.Queries;
using Microsoft.AspNetCore.JsonPatch;
using MySql.Data.MySqlClient;

namespace E_commerce.Infrastructure.repositories
{
    public class ConversationRepository: BaseRepository<_Conversation>, IConversationRepository
    { 
        ///<summary>
        /// Constructor
        /// </summary>
        public ConversationRepository(ILogger logger, IUnitOfWork unitOfWork)
            :base(unitOfWork, logger)
        { }

        /// <summary>
        /// Kiểm tra tính hợp lệ của Conversation
        /// </summary>
        private void ValidateConversation(_Conversation conversation){
            if(conversation == null)
                throw new ValidationException("Conversation không được để trống");

            if(string.IsNullOrWhiteSpace(conversation.conversation_name))
                throw new ValidationException("Tên xếp hạng không được để trống");
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ ID của Conversation
        /// </summary>
        private void ValidateConversationId(string id){
            if(string.IsNullOrWhiteSpace(id))
                throw new ValidationException("ID không được bỏ trống");
        }

        /// <summary>
        /// Lấy danh sách các conversation
        /// </summary>
        public override async Task<IReadOnlyList<_Conversation>> GetAllAsync(){
            try{
                var query = ConversationQueries.GetAll;
                var conversations = await Connection.QueryAsync<_Conversation>(query, transaction: Transaction);
                return conversations.ToList();
            }
            catch(Exception ex){
                _logger.Error("Lỗi khi lấy danh sách Conversation.");
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Lấy conversation dựa trên ID
        /// </summary>
        public override async Task<_Conversation> GetByIdAsync(string id){
            
            ValidateConversationId(id);

            try
            {
                var conversation = await Connection
                    .QueryFirstOrDefaultAsync<_Conversation>(
                        ConversationQueries.findByID, 
                        new { conversation_id = id},
                        transaction: Transaction
                    );
                
                if(conversation == null)
                    throw new ResourceNotFoundException($"Không tìm thấy Conversation với ID: {id}");

                return conversation;
                
            }
            catch(MySqlException ex){

                _logger.Error($"Database error when retrieving ConversationId: {id}, Error Number: {ex.Number}, Message:{ex.Message}", ex);
                throw new DatabaseException("Lỗi khi truy vấn Conversation dựa trên ID");
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error retrieving Conversations with ID: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }


        /// <summary>
        /// Thêm một Conversation mới vào cơ sở dữ liệu
        /// </summary>
        public override async Task<string> AddAsync(_Conversation conversation){

            try{
                var query = ConversationQueries.Add;
                var result = await Connection.ExecuteAsync(query, conversation, transaction: Transaction);
                return result.ToString();
            }
            catch(MySqlException ex){
                _logger.Error($"Database error when adding new Conversation: {ex.Number}, Message:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error adding new Conversation: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Cập nhật conversation trong cơ sở dữ liệu
        /// </summary>
        public override async Task<string> UpdateAsync(_Conversation entity){
            
            ValidateConversation(entity);

            try{
                var result = await Connection.ExecuteAsync(
                    ConversationQueries.UpdateByID, entity, transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy Conversation: {entity.conversation_id}");

                return "SUCCESS";
            }
            catch(MySqlException ex){
                _logger.Error($"Database error when updating Conversation \n Error number:{ex.Number} \nMessage:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex,"Lỗi khi cập nhật Conversation vào cơ sở dữ liệu");
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error updating Conversation: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Xóa Conversation dựa trên ID
        /// </summary>
        public override async Task<string> DeleteAsync(string id){

            ValidateConversationId(id);

            try{
                var result = await Connection.ExecuteAsync(
                    ConversationQueries.DeleteByID,
                    new { conversation_id = id},
                    transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy Conversation: {id}");

                return "SUCCESS";
            }
            catch (MySqlException ex)
            {
                // Ghi đầy đủ thông tin lỗi bao gồm số lỗi
                 _logger.Error($"Database error when deleting Conversation with ID {id} MySQL error #{ex.Number}: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Error deleting Conversation with ID {id}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public override async Task<string> PatchAsync(string id, JsonPatchDocument<_Conversation> patchDoc){
            
            ValidateConversationId(id);

            if(patchDoc == null)
                throw new ValidationException("Dữ liệu cần cập nhật không được bỏ trống");
            
            try{
                
                //Kiểm thử ConversationId có tồn tại không
                var Conversation = await GetByIdAsync(id);
                if(Conversation == null)
                    throw new ResourceNotFoundException($"Không tìm thấy Conversation với ID: {id}");
                
                //Áp dụng các thay đổi
                patchDoc.ApplyTo(Conversation);

                //Cập nhât cơ sở dữ liệu
                var result = await Connection.ExecuteAsync(
                    ConversationQueries.PatchConversation, Conversation, transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Conversation: {id}");

                return "SUCCESS";
            }
            catch (MySqlException ex)
            {
                // Ghi đầy đủ thông tin lỗi bao gồm số lỗi
                _logger.Error($"MySQL error #{ex.Number}: {ex.Message}", ex);
                throw new DatabaseException("Lỗi khi cập nhật một phần thông tin vai trò");
            }
            catch(Exception ex){
                _logger.Error($"Error patching Conversation: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
    }
}