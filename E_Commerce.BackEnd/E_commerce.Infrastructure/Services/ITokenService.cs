using E_commerce.Application.DTOs.Common;

namespace E_commerce.Infrastructure.Services
{
    public interface ITokenService
    {
        //Kiểm tra token còn hạn không
        public Task<bool> CheckIfTokenIsExpired(string token);

        //Kiểm tra xem token có hợp lệ không
        public Task<bool> CheckIfTokenIsValid(string token);

        //Tạo Access token
        public Task<TokenDTO> GenerateToken(AccountInforDTO accountInforDTO, int time = 24);

        //Thu hồi access token
        //public Task<bool> RevokeAccessToken(string token);

        //Thu hồi refresh token
        //public Task<bool> RevokeRefreshToken(string token);
    }
}