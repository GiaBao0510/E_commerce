using E_commerce.Application.DTOs.Requests;

namespace E_commerce.Infrastructure.Services
{
    public interface IOTPAuthenServices
    {
        //Gửi mã OTP đến mail - Mailjet
        //Task<bool> AddOTP_EmailToRedis_Mailjet(string email);

        //Gửi mã OTP đến mail
        Task AddOTP_EmailToRedis_Mailtrap(string email);

        //Gửi mã OTP thông qua sms
        Task AddOTP_PhoneNumToRedis_Vonage(string phoneNum);
        
        //Xác thực mã OTP từ email(key)
        Task<bool> VerifyOTP_Email(VerifyOTP_DTO info);
    }
}