namespace E_commerce.Application.Common.Interface
{
    public interface IImproveDescriptionStrategyFactory
    {
        IImproveDescription GetImproveDescriptionStrategy(string topicType);
        IEnumerable<string> GetAvailableTopicTypes();
    }
}