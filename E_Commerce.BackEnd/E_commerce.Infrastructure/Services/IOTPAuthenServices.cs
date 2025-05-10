using E_commerce.Application.DTOs.Requests;

namespace E_commerce.Infrastructure.Services
{
    public interface IOTPAuthenServices
    {
        //Gửi mã OTP đến mail - Mailjet
        Task<bool> AddOTP_EmailToRedis_Mailjet(string email);

        //Gửi mã OTP đến mail
        Task<bool> AddOTP_EmailToRedis_Mailtrap(string email);
        
        //Xác thực mã OTP từ email(key)
        Task<bool> VerifyOTP_Email(VerifyOTP_DTO info);
    }
}