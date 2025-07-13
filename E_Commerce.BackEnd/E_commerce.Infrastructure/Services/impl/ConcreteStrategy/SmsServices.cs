using E_commerce.Application.Application;
using E_commerce.Application.DTOs.Requests;
using E_commerce.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;

namespace E_commerce.Infrastructure.Services.impl.ConcreteStrategy
{
    public class SmsServices : ISendVerificationMessage
    {
        // Khởi tạo các biến cần thiết
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly VonageClient _vonageClient;

        //Khơi tạo
        public SmsServices(
            IConfiguration configuration,
            ILogger logger
        )
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            //Thiết lập thông tin Vonage
            var credentials = Credentials.FromApiKeyAndSecret(
                _configuration["Authentication:Vonage:ApiKey"],
                _configuration["Authentication:Vonage:ApiSecret"]
            );

            _vonageClient = new VonageClient(credentials);
        }

        //Gán loại dịch vụ
        public string ServiceTypeName => "SMS";

        //Gửi mã xác thực
        public async Task SendVerificationCodeAsync(MessageModel messageModel)
        {
            try
            {
                var response = await _vonageClient.SmsClient.SendAnSmsAsync(new SendSmsRequest
                {
                    From = messageModel.From,
                    To = messageModel.To,
                    Text = messageModel.text
                });

                //Kiểm tra kết quả gửi
                if (response.Messages[0].Status == "0")
                {
                    _logger.Info($"Gửi SMS thành công: {response.Messages[0].StatusCode}");
                }
                else
                {
                    _logger.Error($"Gửi SMS thất bại: {response.Messages[0].ErrorText}");
                    throw new DetailsOfTheException(new Exception(response.Messages[0].ErrorText), "Lỗi khi gửi mã xác thực qua SMS");
                }
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi gửi mã xác thực qua SMS: {ex.Message}");
                throw new DetailsOfTheException(ex, "Lỗi khi gửi mã xác thực qua SMS");
            }
        }
    }
}