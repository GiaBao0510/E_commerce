using E_commerce.Api.Model;
using E_commerce.Application.DTOs.Requests;
using E_commerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    public class OTPAuthenticationController: BaseApiController
    {
        #region ===[private properties]===
        private readonly IOTPAuthenServices _otpAuthenServices;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public OTPAuthenticationController(IOTPAuthenServices otpAuthenServices) =>
            _otpAuthenServices = otpAuthenServices ?? throw new ArgumentNullException(nameof(otpAuthenServices));
        
        // Hàm gửi mã otp đến mail - mailjet
        [HttpPost("mailjet/send-otp-email")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddOTP_EmailToRedis_Mailjet([FromQuery] string email){
            bool result = await _otpAuthenServices.AddOTP_EmailToRedis_Mailjet(email);
            return Success(result, "Gửi mã OTP thành công"); 
        }

        // Hàm gửi mã otp đến mail - mailtrap
        [HttpPost("mailtrap/send-otp-email")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddOTP_EmailToRedis_Mailtrap([FromQuery] string email){
            bool result = await _otpAuthenServices.AddOTP_EmailToRedis_Mailtrap(email);
            return Success(result, "Gửi mã OTP thành công"); 
        }

        // Hàm xác thực mã otp từ mail
        [HttpPost("verify-otp-email")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerifyOTP_Email([FromBody] VerifyOTP_DTO info){
            bool result = await _otpAuthenServices.VerifyOTP_Email(info);
            if(!result)
                return InternalError("Lỗi nội bộ");
            return Success(result, "Xác thực mã OTP thành công");
        }
    }
}