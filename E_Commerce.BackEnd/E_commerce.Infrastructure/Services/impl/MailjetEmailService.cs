using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.Core.Exceptions;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Extensions.Configuration;

namespace E_commerce.Infrastructure.Services.impl
{
    public class MailjetEmailService: IMailjetService
    {
        #region ===[Private Member]===
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public MailjetEmailService(
            IConfiguration configuration,
            ILogger logger
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<bool> SendEmailAsync(_EmailModel emailModel)
        {
            var apiKey = _configuration["Authentication:Mailjet:APIKEY_PUBLIC"];
            var apiSecret = _configuration["Authentication:Mailjet:APIKEY_PRIVATE"];
            var client = new MailjetClient(apiKey, apiSecret);
            
            //Thiết lập nội dung cần gửi
            var email = new TransactionalEmailBuilder() 
                .WithFrom(new SendContact(_configuration["Authentication:Mailjet:Mail"],"E-Commerce"))
                .WithSubject(emailModel.Subject)
                .WithHtmlPart(emailModel.htmlMessage)
                .WithTo(new SendContact(emailModel.EmailTo))
                .Build();
                
            try{
                //Gửi mail
                var response = await client.SendTransactionalEmailAsync(email);
                _logger.Info($"Gửi mail thành công:{response.Messages}");
                return response.Messages != null && response.Messages.Any();
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Lỗi khi gửi mail : {ex.Message}");
                throw new DetailsOfTheException(ex, "Lỗi khi gửi mail");
            }
        }
    }

}