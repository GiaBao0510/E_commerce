using E_commerce.Application.Application;

namespace E_commerce.Infrastructure.Services.impl.Factory
{
    public class SendVerificationMessageStrategyFactory : ISendVerificationMessageStrategyFactory
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<ISendVerificationMessage> _strategies;

        //Khởi tạo
        public SendVerificationMessageStrategyFactory(
            IEnumerable<ISendVerificationMessage> strategies,
            ILogger logger
        )
        {
            _strategies = strategies;
            _logger = logger;
        }

        //Lấy chiến lượt gửi tin nhắn theo dạng dịch vụ
        public ISendVerificationMessage GetSendVerificationMessageStrategy(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                _logger.Error("Loại dịch vụ không được để trống."); 
                throw new Core.Exceptions.InvalidOperationException("Loại dịch vụ không được để trống.");   
            }

            var strategies = _strategies.FirstOrDefault(s => s.ServiceTypeName.Equals(type, StringComparison.OrdinalIgnoreCase));

            return strategies ??
                throw new Core.Exceptions.InvalidOperationException($"Không tìm thấy chiến lược gửi tin nhắn cho loại dịch vụ: {type}");
        }

        //Lấy danh sách các loại dịch vụ có sẵn
        public IEnumerable<string> GetAvailableServiceTypes()
        {
            return _strategies.Select(s => s.ServiceTypeName);
        } 
    }
}