using E_commerce.Api.Model;
using E_commerce.Application.DTOs.Common;
using E_commerce.Application.DTOs.Requests;
using E_commerce.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AccountController: BaseApiController
    {
        #region ===[Private Member]===
        private readonly IAccountServices accountServices;
        private readonly IConfiguration configuration;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public AccountController(
            IAccountServices _accountServices,
            IConfiguration _configuration
        ){
            accountServices = _accountServices ?? throw new ArgumentNullException(nameof(_accountServices));
            configuration = _configuration ?? throw new ArgumentNullException(nameof(_configuration));
        }

        //Đăng nhập thông thường
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> NormalLogin([FromBody]LoginDTO login){
            var result = await accountServices.Login(login);
            
            TokenDTO accessToken = result.Item1;
            TokenDTO refreshToken = result.Item2;

            //Tạo cấu hình CookieOption
            var accessTokenCookieOption = new CookieOptions{
                Path = "/",
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(3)
            };

            var refreshTokenCookieOption = new CookieOptions{
                Path = "/",
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            };

            //Gửi đến người dùng
            Response.Cookies.Append("access_token", accessToken.token, accessTokenCookieOption);
            Response.Cookies.Append("refresh_token", refreshToken.token, refreshTokenCookieOption);

            return Success("Đăng nhập thành công");
        }
    }
}