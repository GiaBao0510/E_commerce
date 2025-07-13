namespace E_commerce.Infrastructure.Services
{
    public interface ISendVerificationMessageStrategyFactory
    {
        //Lấy chiến lượt gửi tin nhắn theo dạng dịch vụ
        ISendVerificationMessage GetSendVerificationMessageStrategy(string type);

        //Lấy danh sách các loại dịch vụ có sẵn
        IEnumerable<string> GetAvailableServiceTypes();
    }
}