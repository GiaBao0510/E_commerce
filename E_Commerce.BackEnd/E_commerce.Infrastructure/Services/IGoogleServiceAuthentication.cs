using E_commerce.Application.DTOs.Requests;
using E_commerce.Core.Entities;

namespace E_commerce.Infrastructure.Services
{
    public interface IGoogleServiceAuthentication
    {
        /// <summary>
        /// Xác thực token từ google
        /// </summary>
        public Task<(string, string)> VerifyGoogleToken(string token);

        /// <summary>
        /// Kiểm tra xem email này của khách hàng đã có trong csdl hay chưa. Nếu có thì lấy thông tin người dùng
        /// </summary> 
        public Task<_User> CheckVerifyAccountViaEmail(GoogleVerificationDTO googleVerificationDTO);

        /// <summary>
        /// Đăng nhập thông qua email
        /// </summary> 
        public Task<_User> Login(string? name, string? email);
    }
}