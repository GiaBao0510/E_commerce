using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace E_commerce.Infrastructure.RateLimiting
{
    public interface IRateLimitingService
    {
        /// <summary>
        /// Cấu hình rate limiting dựa trên trạng thái xác thực
        /// </summary>
        /// <param name="context">HTTP context của request</param>
        /// <returns>Danh sách quy tắc rate limiting áp dụng cho request</returns>
        IEnumerable<RateLimitRule> GetRateLimitRules(HttpContext context);
        
        /// <summary>
        /// Đăng ký các dịch vụ rate limiting vào DI container
        /// </summary>
        /// <param name="services">Service collection</param>
        void RegisterRateLimitingServices(IServiceCollection services);
    }
}