using E_commerce.Application.DTOs.Requests;

namespace E_commerce.Infrastructure.Services
{
    public interface ISendVerificationMessage
    {
        public string ServiceTypeName {get; }
        public Task SendVerificationCodeAsync(MessageModel messageModel);
    }
}