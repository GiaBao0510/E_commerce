using E_commerce.Application.Application;
using E_commerce.Application.Common.Interface;
using E_commerce.Core.Exceptions;

namespace E_commerce.Application.Common.Behavious.Context
{
    /// <summary>
    /// Đây là lớp ngữ cảnh dựa trên chiến lược để cải thiện mô tả.
    /// Nó sử dụng một nhà máy chiến lược để lấy chiến lược phù hợp dựa trên loại chủ đề.
    /// </summary>
    public class ImproveDescriptionContext : IImproveDescriptionContext
    {
        private readonly IImproveDescriptionStrategyFactory _strategyFactory;
        private readonly ILogger _logger;

        public ImproveDescriptionContext(IImproveDescriptionStrategyFactory strategyFactory, ILogger logger)
        {
            _strategyFactory = strategyFactory;
            _logger = logger;
        }

        // 
        public async Task<string> ImproveDescriptionAsync(string topicType, string input)
        {
            try
            {
                if (string.IsNullOrEmpty(input))
                    throw new Core.Exceptions.InvalidOperationException("Dữ liệu đầu vào không được để trống.");

                var strategy = _strategyFactory.GetImproveDescriptionStrategy(topicType);

                _logger.Info($"Đang cải thiện mô tả với loại chủ đề: {topicType}");
                return await strategy.ImproveDescription_ByGPT(input);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi cải thiện mô tả: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public IEnumerable<string> GetAvailableTopicTypes()
        {
            return _strategyFactory.GetAvailableTopicTypes();
        }
    }
}