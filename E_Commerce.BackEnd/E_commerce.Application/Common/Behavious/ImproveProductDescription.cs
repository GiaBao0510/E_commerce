using E_commerce.Application.Application;
using E_commerce.Application.Common.Interface;
using E_commerce.Application.Constants;
using E_commerce.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace E_commerce.Application.Common.Behavious
{
    public class ImproveProductDescription : IImproveDescription
    {
        #region ===[Private Fields]===
        private readonly ILogger _logger;
        private readonly ChatClient _chatClient;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public ImproveProductDescription(
            IConfiguration configuration,
            ILogger logger
        )
        {
            _logger = logger;

            var modelName = configuration["AI_Models:OpenAI:Models:GPT_4o_MINI"];
            var ApiKey = configuration["AI_Models:OpenAI:ApiKey"];
            _chatClient = new ChatClient(modelName, ApiKey); // Khởi tạo client chat với model và API key
        }

        /// <summary>
        /// Loại chủ đề
        /// </summary>
        public string TopicType => "ImproveProductDescription";

        /// <summary>
        /// Phương thức cải thiện mô tả sản phẩm bằng GPT
        /// </summary>
        public async Task<string> ImproveDescription_ByGPT(string input)
        {
            try
            {
                string inputWithTheme = string.Format(ThemeSampleToImprove.ImproveProductDescription, input); // Kết hợp đầu vào với mẫu cải thiện mô tả loại sản phẩm
                var response = await _chatClient.CompleteChatAsync(inputWithTheme);   // Gửi yêu cầu hoàn thành chat với đầu vào

                return response.Value.Content[0].Text;
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi cải thiện mô tả sản phẩm bằng chatGPT: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
    }
}