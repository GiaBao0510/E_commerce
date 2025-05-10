using E_commerce.Application.DTOs.Common;
using E_commerce.Application.DTOs.Requests;
using E_commerce.Core.Entities;

namespace E_commerce.Infrastructure.Services
{
    public interface IGoogleServiceAuthentication
    {
        /// <summary>
        /// Xác thực token từ google
        /// </summary>
        public Task<(string?, string?)> VerifyGoogleToken(GoogleVerificationDTO googleVerificationDTO);

        /// <summary>
        /// Kiểm tra xem email này của khách hàng đã có trong csdl hay chưa. Nếu có thì lấy thông tin người dùng
        /// </summary> 
        public Task<_User> CheckVerifyAccountViaEmail(string email);

        /// <summary>
        /// Kiểm tra email người dùng cố tồn tại không, Nếu chưa thì tạo mới
        /// </summary> 
        public Task<_User> GetOrCreateUser(string email, string name); 

        /// <summary>
        /// Đăng nhập thông qua email
        /// </summary> 
        public Task<(TokenDTO, TokenDTO)> Login(GoogleVerificationDTO googleVerificationDTO);
    }
}