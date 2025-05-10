using E_commerce.Core.Entities;

namespace E_commerce.Infrastructure.Services
{
    public interface IMailtrapService
    {
        public Task<bool> SendEmailAsync(_EmailModel emailModel);
    }
}