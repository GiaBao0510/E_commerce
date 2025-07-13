using E_commerce.Application.Application;
using E_commerce.Application.Common.Interface;
using E_commerce.Core.Exceptions;

namespace E_commerce.Application.Common.Behavious.Strategy
{
    public class ImproveDescriptionStrategyFactory : IImproveDescriptionStrategyFactory
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IImproveDescription> _strategies;

        //Hàm khởi tạo
        public ImproveDescriptionStrategyFactory(
            IEnumerable<IImproveDescription> strategies,
            ILogger logger
        )
        {
            _strategies = strategies;
            _logger = logger;
        }

        public IImproveDescription GetImproveDescriptionStrategy(string topicType) 
        {
            try
            {
                var Strategy = _strategies.FirstOrDefault(s => s.TopicType.Equals(topicType, StringComparison.OrdinalIgnoreCase));

                return Strategy ??
                    throw new Core.Exceptions.InvalidOperationException($"Không tìm thấy chiến lược cải thiện mô tả cho chủ đề: {topicType}");
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi lấy chiến lược cải thiện mô tả: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public IEnumerable<string> GetAvailableTopicTypes()
        {
            return _strategies.Select(s => s.TopicType);
        }
    }
}