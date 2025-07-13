using E_commerce.Core.Entities;

namespace E_commerce.Application.Application
{
    public interface IGroupChatRepository : IRepository<_GroupChat>
    {
        /// <summary>
        /// Danh sách các cuộc hội thoại của người dùng dựa trên UserID
        /// </summary>
        public Task<IReadOnlyList<_Conversation>> GetConversationByUserID(string uid);

        /// <summary>
        /// Danh sách các người dùng trong nhóm dựa trên ConversationID
        /// </summary>
        public Task<IReadOnlyList<_User>> GetUserByConversationID(string conversation_id);
        
        /// <summary>
        /// Tạo cuộc trò chuyện cá nhân giữa 2 người
        /// </summary>
        //public Task<>

        /// <summary>
        /// Tạo nhóm chat giữa 3 người trở lên
        /// </summary>


        /// <summary>
        /// Kiểm tra người dùng có trong nhóm chat hay không
        /// </summary>

        /// <summary>
        /// thêm người dùng vào nhóm chat
        /// </summary>
        
        /// <summary>
        /// Xóa người dùng khỏi nhóm chat
        /// </summary>
    }
}