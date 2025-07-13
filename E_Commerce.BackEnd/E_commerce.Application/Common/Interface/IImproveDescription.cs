namespace E_commerce.Application.Common.Interface
{
    public interface IImproveDescription
    {
        Task<string> ImproveDescription_ByGPT(string input);
        public string TopicType { get; } 
    }
}