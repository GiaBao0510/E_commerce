using E_commerce.Application.Application;
using E_commerce.Application.DTOs.Requests;
using E_commerce.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace E_commerce.Infrastructure.Services.impl.ConcreteStrategy
{
    public class MailtrapSerrvices: ISendVerificationMessage
    {
        #region ===[Private Member]===
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public MailtrapSerrvices(
            IConfiguration configuration,
            ILogger logger
        ){
            _configuration = configuration;
            _logger = logger;
        }

        //Gắn loại dịch vụ
        public string ServiceTypeName => "MailTrap";

        //Gửi mail
        public async Task SendVerificationCodeAsync(MessageModel messageModel)
        {

            //Thiết lập nội dung cần gửi đi
            var client = new RestClient(_configuration["Authentication:MailTrap:Host"]);
            var res = new RestRequest();
            res.AddHeader("Authorization", "Bearer " + _configuration["Authentication:MailTrap:ApiToken"]);
            res.AddHeader("Content-Type", "application/json");
            res.AddJsonBody(new
            {
                from = new { email = messageModel.From, name = "E-commerce" },
                to = new[] { new { email = messageModel.To } },
                subject = messageModel.Subject,
                html = messageModel.text
            });

            try
            {
                var req = await client.PostAsync(res);

                if (req.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.Info($"Gửi mail thành công:{req.Content}");
                }
                else
                {
                    _logger.Error($"Gửi mail thất bại:{req.Content}");
                    throw new DetailsOfTheException(new Exception(req.Content), "Lỗi khi gửi mail");
                }
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi gửi mail : {ex.Message}");
                throw new DetailsOfTheException(ex, "Lỗi khi gửi mail");
            }
        }
        
    }
}