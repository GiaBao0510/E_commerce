using E_commerce.Core.Entities;

namespace E_commerce.Application.Application
{
    public interface IGroupChatRepository: IRepository<_GroupChat>
    {
        /// <summary>
        /// Danh sách các cuộc hội thoại của người dùng dựa trên UserID
        /// </summary>
        public Task<IReadOnlyList<_Conversation>> GetConversationByUserID(string uid);

        /// <summary>
        /// Danh sách các người dùng trong nhóm dựa trên ConversationID
        /// </summary>
        public Task<IReadOnlyList<_User>> GetUserByConversationID(string conversation_id);
    }
}