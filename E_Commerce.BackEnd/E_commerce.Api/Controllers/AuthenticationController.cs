using System.Net;
using E_commerce.Api.Model;
using E_commerce.Application.Application;
using E_commerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    /// <summary>
    /// Có thể xác thực token từ google, facebook, github,...
    /// </summary>
    public class authController: BaseApiController
    {
        #region ===[Private properties]===
        private readonly IGoogleServiceAuthentication googleServiceAuthentication;
        private readonly Application.Application.ILogger logger;
        private readonly LinkGenerator linkGenerator;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public authController(
            IGoogleServiceAuthentication _googleServiceAuthentication,
            LinkGenerator _linkGenerator,
            Application.Application.ILogger _logger
        )
        {
            googleServiceAuthentication = _googleServiceAuthentication ??
                throw new ArgumentNullException(nameof(_googleServiceAuthentication));
            linkGenerator = _linkGenerator ??
                throw new ArgumentNullException(nameof(_linkGenerator));
            logger = _logger ??
                throw new ArgumentNullException(nameof(_logger));
        }

        //Thiết lập tuy chọn cookies
        private CookieOptions CreateSecureCookieOptions(DateTime expires){
            return new CookieOptions{
                Path = "/",
                HttpOnly = false,
                Secure = false, 
                SameSite = SameSiteMode.Lax,            //Trong môi trường kiểm thử
                Expires = expires
            };            
        }

        //Kiểm tra xem cấu hình có hoạt động đúng trong môi trường devveloper
        [HttpGet("test-auth")]
        [AllowAnonymous]
        public IActionResult TestAuth()
        {
            Response.Cookies.Append(
                "test_cookie",
                "test_value",
                new CookieOptions
                {
                    Path = "/",
                    HttpOnly = false,
                    Secure = false, // Chỉ sử dụng trong môi trường phát triển
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(5)
                }
            );

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Test cookie created successfully.",
                Meta = new MetaData
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    RequestId = HttpContext.TraceIdentifier,
                    Timestamp = DateTime.UtcNow
                }
            });
        }

        //Đăng nhập thông qua google
        [HttpGet("signin-google")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginGoogle([FromQuery] string returnUrl)
        {
            //Đảm bảo returnUrl hợp lệ và thuộc danh sách Allowed Origins
            if (string.IsNullOrEmpty(returnUrl) || !Uri.IsWellFormedUriString(returnUrl, UriKind.Absolute))
                returnUrl = "https://localhost:3000/auth/callback";

            logger.Info($"starting google login with returnUrl: {returnUrl}");

            //tạo thuộc tính xác thực cho Google
            var authProperties = new AuthenticationProperties
            {
                //Chỉ định URL để chuyển hướng sau khi xác thực thành công
                RedirectUri = Url.Action(
                    "GoogleCallback",
                    "auth",
                    values: new { returnUrl }
                ),

                //Chỉ định các tham số bổ sung cho xác thực
                Items = {
                    {"returnUrl", returnUrl}
                }
            };

            //Chuyển hướng đến Google Authentication
            return Challenge(authProperties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("login-google-callback")]
        [AllowAnonymous]
        [ActionName("GoogleCallback")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GoogleCallBack([FromQuery] string returnUrl) {
            try
            {
                logger.Info($"Google callback with returnUrl: {returnUrl}");
                
                //Thực hiện lấy kết quả xác thực từ Google
                var authenticationResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                //Nếu thất bại thì trả về lỗi 404
                if (!authenticationResult.Succeeded)
                {
                    logger.Error($"Google authentication failed: {authenticationResult.Failure.Message}");
                    return Redirect($"{returnUrl}?error={WebUtility.UrlEncode("Authentication failed")}");
                }

                //Lấy claims từ google
                var claims = authenticationResult.Principal;
                if (claims == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Claims is null.",
                        Meta = new MetaData
                        {
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            RequestId = HttpContext.TraceIdentifier,
                            Timestamp = DateTime.UtcNow
                        }
                    });
                }

                //Tạo và lấy access_token và refresh_token
                var (access_token, refresh_token) = await googleServiceAuthentication.Login(claims);

                //Gửi token lên client thông qua cookie
                Response.Cookies.Append(
                    "access_token",
                    access_token.token,
                    CreateSecureCookieOptions(access_token.expiration)
                );
                Response.Cookies.Append(
                    "refresh_token",
                    refresh_token.token,
                    CreateSecureCookieOptions(refresh_token.expiration)
                );

                //Trả về bên FE
                return Redirect(returnUrl ?? "http://localhost:3000/home");
            }
            catch (Exception ex)
            {
                //Lỗi 500
                return Redirect($"{returnUrl ?? "http://localhost:3000/login"}?error={WebUtility.UrlEncode(ex.Message)}");
            }
        }
    }
}