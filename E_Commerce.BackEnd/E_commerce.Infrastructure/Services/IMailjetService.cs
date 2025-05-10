using E_commerce.Core.Entities;

namespace E_commerce.Infrastructure.Services
{
    public interface IMailjetService
    {
        public Task<bool> SendEmailAsync(_EmailModel emailModel);
    }
}