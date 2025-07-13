namespace E_commerce.Application.DTOs.Requests
{
    public class MessageModel
    {
        public string Subject { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string text { get; set; } = string.Empty;
    }
}