using System.Security.Claims;
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
        /// Đăng nhập thông qua email
        /// </summary> 
        public Task<(TokenDTO, TokenDTO)> Login(ClaimsPrincipal? claimsPrincipal);
    } 
}