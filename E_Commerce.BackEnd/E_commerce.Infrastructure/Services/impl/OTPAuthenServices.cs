using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_commerce.Application.Application;
using E_commerce.Application.DTOs.Requests;
using E_commerce.Core.Entities;
using E_commerce.Core.Exceptions;
using E_commerce.Infrastructure.Templates.Email;
using E_commerce.Infrastructure.Utils;
using Microsoft.Extensions.Configuration;

namespace E_commerce.Infrastructure.Services.impl
{
    public class OTPAuthenServices: IOTPAuthenServices
    {
        #region ===[private properties]===
        private readonly IRedisServices _redisServices;
        private readonly IMailjetService _mailjetEmailService;
        private readonly IMailtrapService _mailtrapEmailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public OTPAuthenServices(
            IRedisServices redisServices,
            IMailjetService mailjetEmailService,
            IMailtrapService mailtrapEmailService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger logger
        )
        {
            _redisServices = redisServices ?? throw new ArgumentNullException(nameof(redisServices));
            _mailjetEmailService = mailjetEmailService ?? throw new ArgumentNullException(nameof(mailjetEmailService));
            _mailtrapEmailService = mailtrapEmailService ?? throw new ArgumentNullException(nameof(mailtrapEmailService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Hàm kiểm tra hợp lệ
        /// </summary>
        private void CheckValid(string content, string message = ""){
            if(string.IsNullOrEmpty(content))
                throw new Core.Exceptions.InvalidOperationException($"{message} không được bỏ trống");
        }

        /// <summary>
        /// Hàm giới hạn gửi mã OTP cho email, Nếu quá 5 lần sẽ hiển thông báo
        /// </summary>
        private async Task<bool> HasExceededRateLimit(string email){
            try{

                string rateLimitKey = $"opt:ratelimit:{email}";             //Key
                int count = await _redisServices.Get<int>(rateLimitKey);    //Value đếm

                //Số lần tối đa gửi mã OTP trong 1 giờ
                int maxCount = 5; //Giới hạn gửi mã OTP trong 1 giờ

                //Nếu số lần vượt quá giói hạn thì trả về true
                if(count >= maxCount)
                    return true;

                //Kiểm tra trong count có giá trị không
                if(count == 0){
                    //Nếu không có thì thêm vào redis với giá trị 1 và thời gian sống là 10 phút
                    await _redisServices.Set(rateLimitKey, 1, TimeSpan.FromHours(1));
                }
                else{
                    //Nếu có thì tăng giá trị lên 1
                    await _redisServices.Set(rateLimitKey, count + 1, TimeSpan.FromMinutes(10));
                }

                return false;
            }
            catch(Exception ex){
                _logger.Error($"Lỗi khi kiểm tra giới hạn gửi mã OTP: {ex.Message}");
                throw new DetailsOfTheException(ex, "Lỗi khi kiểm tra giới hạn gửi mã OTP");
            }
        }

        /// <summary>
        /// Hàm điều kiển trước khi gửi mã OTP đến mail
        /// </summary>
        private async Task BeforeSendingOTP(string email){
            try{
                //Kiểm tra email không được bỏ trống
                CheckValid(email,"Email");

                //Kiểm tra email người dùng có tồn tại không
                bool findByUserEmail = await _unitOfWork.users.IsUserEmailExists(email);
                if(!findByUserEmail)
                    throw new ResourceNotFoundException("Email không tồn tại trong hệ thống");
                
                //Kiểm tra xem mã OTP dựa trên email có còn hạn không
                bool isKeyExists = await _redisServices.KeyExists(email);
                if(isKeyExists)
                    throw new Core.Exceptions.InvalidOperationException("Mã OTP đã được gửi đến email này, vui lòng kiểm tra lại email của bạn");
                
                //Kiểm tra xem có vượt quá giới hạn gửi mã OTP không
                if(await HasExceededRateLimit(email))
                    throw new TooManyRequestsException("Vượt quá giới hạn gửi mã OTP, vui lòng thử lại sau 1 giờ");
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error($"Lỗi chi tiết khi gửi mail {ex.Message} ", ex);
                throw new DetailsOfTheException(ex, "Lỗi chi tiết khi gửi mail");
            }
        }

        /// <summary>
        /// Gửi mã OTP đến mail
        /// </summary>
        public async Task<bool> AddOTP_EmailToRedis_Mailjet(string email){
            try{
                //Kiểm tra trước khi gửi mã OTP đến mail
                await BeforeSendingOTP(email);
                
                //Lưu thông tin OTP vào redis
                string otp = CodeGenerator.GenerateRandomNumber(6);
                await _redisServices.Set<string>(email, otp, TimeSpan.FromMinutes(5));

                //Gửi mã OTP đến mail
                string template = VerificationForm.OTPcodeVerificationForm(otp);
                _EmailModel emailModel = new _EmailModel{
                    Subject = "Mã xác thực OTP",
                    EmailTo = email,
                    htmlMessage = template,
                };

                return await _mailjetEmailService.SendEmailAsync(emailModel);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Lỗi chi tiết khi gửi mail {ex.Message} ", ex);
                throw new DetailsOfTheException(ex, "Lỗi chi tiết khi gửi mail");
            }
        }

        /// <summary>
        /// Gửi mã OTP đến mail - Mailtrap
        /// </summary>
        public async Task<bool> AddOTP_EmailToRedis_Mailtrap(string email)
        {
            try{
                //Kiểm tra trước khi gửi mã OTP đến mail
                await BeforeSendingOTP(email);
                
                //Lưu thông tin OTP vào redis
                string otp = CodeGenerator.GenerateRandomNumber(6);
                await _redisServices.Set<string>(email, otp, TimeSpan.FromMinutes(5));

                //Gửi mã OTP đến mail
                string template = VerificationForm.OTPcodeVerificationForm(otp);
                _EmailModel emailModel = new _EmailModel{
                    Subject = "Mã xác thực OTP",
                    EmailFrom = _configuration["Authentication:MailTrap:EmailTest"],
                    EmailTo = email,
                    htmlMessage = template,
                };

                return  await _mailtrapEmailService.SendEmailAsync(emailModel);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Lỗi chi tiết khi gửi mail {ex.Message} ", ex);
                throw new DetailsOfTheException(ex, "Lỗi chi tiết khi gửi mail");
            }   
        }

        /// <summary>
        /// Xác thực mã OTP từ email(key)
        /// </summary>
        public async Task<bool> VerifyOTP_Email(VerifyOTP_DTO info){
            try{
                //Kiểm tra đầu vào không được bỏ trống
                CheckValid(info.Email,"Email");
                CheckValid(info.OTP,"Mã OTP");

                //Kiểm tra email người dùng có tồn tại không
                bool findByUserEmail = await _unitOfWork.users.IsUserEmailExists(info.Email);
                if(!findByUserEmail)
                    throw new ResourceNotFoundException("Email không tồn tại trong hệ thống");
                
                //Kiểm tra email trong redis có tồn tại không
                bool isKeyExists = await _redisServices.KeyExists(info.Email);
                if(!isKeyExists)
                    throw new Core.Exceptions.InvalidOperationException("Mã OTP đã hết hạn, vui lòng yêu cầu mã mới");

                //So sánh
                string redisOTP = await _redisServices.Get<string>(info.Email);
                if(redisOTP != info.OTP)
                    throw new Core.Exceptions.InvalidOperationException("Mã OTP không chính xác, vui lòng kiểm tra lại mã OTP của bạn");
                
                //Xóa mã OTP trong redis
                return await _redisServices.Remove(info.Email);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Lỗi chi tiết khi gửi mail {ex.Message} ", ex);
                throw new DetailsOfTheException(ex, "Lỗi chi tiết khi gửi mail");
            }  
        }
        
    }
}