namespace E_commerce.Application.DTOs.Requests
{
    public class VerifyOTP_DTO
    {
        public string Email { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
    }
}