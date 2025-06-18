namespace E_commerce.Application.Common.Interface
{
    public interface IImproveDescriptionContext
    {
        Task<string> ImproveDescriptionAsync(string topicType, string input);
        IEnumerable<string> GetAvailableTopicTypes();
    }
}