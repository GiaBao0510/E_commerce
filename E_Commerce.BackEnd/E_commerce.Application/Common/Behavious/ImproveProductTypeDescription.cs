using E_commerce.Application.Application;
using E_commerce.Application.Common.Interface;
using E_commerce.Application.Constants;
using E_commerce.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace E_commerce.Application.Common.Behavious
{
    public class ImproveProductTypeDescription : IImproveDescription, IDisposable
    {
          #region ===[Private Fields]===
        private readonly ILogger _logger;
        private readonly string modelName;
        private readonly string ApiKey;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public ImproveProductTypeDescription(
            IConfiguration configuration,
            ILogger logger
        )
        {
            _logger = logger;
            modelName = configuration["AI_Models:OpenAI:Models:GPT_4o_MINI"];
            ApiKey = configuration["AI_Models:OpenAI:ApiKey"];
        }

        /// <summary>
        /// Loại chủ đề
        /// </summary>
        public string TopicType => "ImproveProductTypeDescription";

        /// <summary>
        /// Phương thức cải thiện mô tả loại sản phẩm bằng GPT
        /// </summary>
        public async Task<string> ImproveDescription_ByGPT(string input)
        {
            try
            {
                var client = new ChatClient(modelName, ApiKey);         // Khởi tạo client chat với model và API key
                string inputWithTheme = $"'{input}'" + ThemeSampleToImprove.ImproveProductTypeInfor; // Kết hợp đầu vào với mẫu cải thiện mô tả loại sản phẩm
                var response = await client.CompleteChatAsync(inputWithTheme);   // Gửi yêu cầu hoàn thành chat với đầu vào

                _logger.Info($"Input: {inputWithTheme}");
                _logger.Info($"Response: {response.Value.Content[0].Text}");
                return response.Value.Content[0].Text;
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi cải thiện mô tả loại sản phẩm bằng chatGPT: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Giải phóng tài nguyên
        /// </summary>
        public void Dispose()
        {
            // Giải phóng tài nguyên nếu cần thiết
            _logger.Info("Giải phóng tài nguyên trong ImproveProductDescription.");
        }
    }
}