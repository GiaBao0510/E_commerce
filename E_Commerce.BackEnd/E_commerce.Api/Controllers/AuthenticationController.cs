using E_commerce.Api.Model;
using E_commerce.Application.DTOs.Common;
using E_commerce.Application.DTOs.Requests;
using E_commerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    /// <summary>
    ///     Có thể xác thực token từ google, facebook, github,...
    /// </summary>
    public class AuthenticationController: BaseApiController
    {
        #region ===[Private properties]===
        private readonly IGoogleServiceAuthentication googleServiceAuthentication;
        private readonly IConfiguration configuration;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public AuthenticationController(
            IGoogleServiceAuthentication _googleServiceAuthentication,
            IConfiguration _configuration
        )
        {
            googleServiceAuthentication = _googleServiceAuthentication ?? 
                throw new ArgumentNullException(nameof(_googleServiceAuthentication));
            configuration = _configuration ?? throw new ArgumentNullException(nameof(_configuration));
        }

        //Thiết lập tuy chọn cookies
        private CookieOptions CreateSecureCookieOptions(DateTime expires){
            bool isDevelop = configuration.GetValue<bool>("Development:IsLocal");

            return new CookieOptions{
                Path = "/",
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = expires
            };            
        }

        //Đăng nhập thông qua google
        [HttpPost("login-google")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginGoogle([FromBody] GoogleVerificationDTO googleVerificationDTO){
            var result = await googleServiceAuthentication.Login(googleVerificationDTO);
            TokenDTO accessToken = result.Item1;
            TokenDTO refreshToken = result.Item2;

            //Tạp cấu hình cookieOption
            var accessTokenCookieOptions = CreateSecureCookieOptions(accessToken.expiration);
            var refreshTokenCookieOptions = CreateSecureCookieOptions(refreshToken.expiration);

            //Gửi đến người dùng
            Response.Cookies.Append("access_token", accessToken.token, accessTokenCookieOptions);
            Response.Cookies.Append("refresh_token", refreshToken.token, refreshTokenCookieOptions);

            return Success("Đăng nhập thông qua google thành công");
        }

        [HttpGet("auth/google-login-callback")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GoogleCallBack(){

            var info = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if(!info.Succeeded)
                return Unauthorized("Xác thực Google thất bại");
            
            var idToken = info.Properties.GetTokenValue("id_token");
            if(string.IsNullOrEmpty(idToken))
                return BadRequest("Không tìm thấy id_token trong thông tin xác thực");
            
            var result = await googleServiceAuthentication.Login(new GoogleVerificationDTO{ IdToken = idToken });
            TokenDTO accessToken = result.Item1;
            TokenDTO refreshToken = result.Item2;

            //Tạp cấu hình cookieOption
            var accessTokenCookieOptions = CreateSecureCookieOptions(accessToken.expiration);
            var refreshTokenCookieOptions = CreateSecureCookieOptions(refreshToken.expiration);

            //Gửi đến người dùng
            Response.Cookies.Append("access_token", accessToken.token, accessTokenCookieOptions);
            Response.Cookies.Append("refresh_token", refreshToken.token, refreshTokenCookieOptions);

            return Success("Đăng nhập thông qua google thành công");
        }
    }
}