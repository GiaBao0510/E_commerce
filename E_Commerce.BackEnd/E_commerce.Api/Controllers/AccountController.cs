using E_commerce.Api.Model;
using E_commerce.Application.DTOs.Common;
using E_commerce.Application.DTOs.Requests;
using E_commerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    [ApiController]
    public class AccountController: BaseApiController
    {
        #region ===[Private Member]===
        private readonly IAccountServices accountServices;
        private readonly ITokenService tokenService;
        private readonly IConfiguration configuration;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public AccountController(
            IAccountServices _accountServices,
            ITokenService _tokenService,
            IConfiguration _configuration
        ){
            accountServices = _accountServices ?? throw new ArgumentNullException(nameof(_accountServices));
            tokenService = _tokenService ?? throw new ArgumentNullException(nameof(_tokenService));
            configuration = _configuration ?? throw new ArgumentNullException(nameof(_configuration));
        }

        //Đăng nhập thông thường
        [HttpPost("login")]
        [HttpOptions("login")]
        [AllowAnonymous]
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
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(3)
            };

            var refreshTokenCookieOption = new CookieOptions{
                Path = "/",
                HttpOnly = false,           //Chỉ môi trường phát triển nên bật false, để các câu lệnh JS có thể truy cập vào
                Secure = false,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            };

            //Gửi đến người dùng
            Response.Cookies.Append("access_token", accessToken.token, accessTokenCookieOption);
            Response.Cookies.Append("refresh_token", refreshToken.token, refreshTokenCookieOption);

            return Success("Đăng nhập thành công");
        }

        //Cấp lại accesstoken
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModelDTO token){

            var result = await tokenService._renewToken(token);

            //Gửi token mới qua cookie
            var accessTokenCookieOption = new CookieOptions{
                Path = "/",
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(3)
            };

            Response.Cookies.Append("access_token", result, accessTokenCookieOption);

            return Success("Cấp lại token thành công");
        }
    }
}