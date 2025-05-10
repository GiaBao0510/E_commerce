using E_commerce.Core.Entities;

namespace E_commerce.Application.Application
{
    public interface IMessageRepository: IRepository<_Message>
    {
        /// <summary>
        /// Lấy danh sách các tin nhắn dựa trên cuộc hội thoại (one to one)
        /// </summary>
        public Task<IReadOnlyList<_Message>> ListOfMessagesByConversationID(string conversation_id);

        /// <summary>
        /// Lấy danh sách các tin nhắn dựa trên Group Chat ID
        /// </summary>
        public Task<IReadOnlyList<_Message>> ListOfMessagesByGroupChatID(string group_id);
    }
}