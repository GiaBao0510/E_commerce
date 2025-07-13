using E_commerce.Application.Application;
using E_commerce.Application.Common.Interface;
using E_commerce.Application.Constants;
using E_commerce.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace E_commerce.Application.Common.Behavious
{
    public class ImprovePromotionalInfor: IImproveDescription, IDisposable
    {
        #region ===[Private Fields]===
        private readonly ILogger _logger;
        private readonly ChatClient _chatClient;

        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public ImprovePromotionalInfor(
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
        public string TopicType => "ImprovePromotionalInformation";

        /// <summary>
        /// Phương thức cải thiện mô tả khuyến mãi bằng GPT
        /// </summary>
        public async Task<string> ImproveDescription_ByGPT(string input)
        {
            try
            {
                string inputWithTheme = string.Format(ThemeSampleToImprove.ImprovePromotionalInfor, input); // Kết hợp đầu vào với mẫu cải thiện mô tả loại sản phẩm
                var response = await _chatClient.CompleteChatAsync(inputWithTheme);   // Gửi yêu cầu hoàn thành chat với đầu vào

                return response.Value.Content[0].Text;
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi cải thiện mô tả khuyến mãi bằng chatGPT: {ex.Message}", ex);
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