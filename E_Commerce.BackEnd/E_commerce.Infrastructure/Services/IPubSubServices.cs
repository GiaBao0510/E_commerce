namespace E_commerce.Infrastructure.Services
{
    public interface IPubSubServices
    {
        /// <summary>
        ///Xuất tin nhắn lên kênh
        /// </summary>
        public Task<bool> PUBLISH_channelAsync(string channel, string message);

        /// <summary>
        /// Đăng ký một kênh cụ thể để nhận tin nhắn
        /// </summary>
        public Task<bool> SUBSCIBE_channelAsync(string channel);

        /// <summary>
        /// Hủy đăng ký một kênh cụ thể để nhận tin nhắn
        /// </summary>
        public Task<bool> UNSUBSCIBE_channelAsync(string channel);
        
        /// <summary>
        /// Liệt kê các kênh đang hoạt động
        /// </summary>
        public Task<bool> PUBSUB_channelAsync(string pattern);
    }
}