using E_commerce.Application.Application;
using E_commerce.Application.DTOs.Requests;
using E_commerce.Core.Entities;
using E_commerce.Core.Exceptions;
using E_commerce.Infrastructure.Templates.Email;
using E_commerce.Infrastructure.Utils;
using Microsoft.Extensions.Configuration;

namespace E_commerce.Infrastructure.Services.impl
{
    public class OTPAuthenServices : IOTPAuthenServices
    {
        #region ===[private properties]===
        private readonly IRedisServices _redisServices;
        private readonly ISendVerificationMessageStrategyFactory _sendVerificationMessage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public OTPAuthenServices(
            IRedisServices redisServices,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ISendVerificationMessageStrategyFactory sendVerificationMessage,
            ILogger logger
        )
        {
            _redisServices = redisServices ?? throw new ArgumentNullException(nameof(redisServices));
            _sendVerificationMessage = sendVerificationMessage ?? throw new ArgumentNullException(nameof(sendVerificationMessage));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Hàm kiểm tra hợp lệ
        /// </summary>
        private void CheckValid(string content, string message = "")
        {
            if (string.IsNullOrEmpty(content))
                throw new Core.Exceptions.InvalidOperationException($"{message} không được bỏ trống");
        }

        /// <summary>
        /// Hàm giới hạn gửi mã OTP cho email hoặc sms, Nếu quá 5 lần sẽ hiển thông báo
        /// </summary>
        private async Task<bool> HasExceededRateLimit(string key)
        {
            try
            {
                string rateLimitKey = $"opt:ratelimit:{key}";             //Key
                int count = await _redisServices.Get<int>(rateLimitKey);    //Value đếm

                //Số lần tối đa gửi mã OTP trong 1 giờ
                int maxCount = 5; //Giới hạn gửi mã OTP trong 1 giờ

                //Nếu số lần vượt quá giói hạn thì trả về true
                if (count >= maxCount)
                    return true;

                //Kiểm tra trong count có giá trị không
                if (count == 0)
                {
                    //Nếu không có thì thêm vào redis với giá trị 1 và thời gian sống là 10 phút
                    await _redisServices.Set(rateLimitKey, 1, TimeSpan.FromHours(1));
                }
                else
                {
                    //Nếu có thì tăng giá trị lên 1
                    await _redisServices.Set(rateLimitKey, count + 1, TimeSpan.FromMinutes(10));
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi khi kiểm tra giới hạn gửi mã OTP: {ex.Message}");
                throw new DetailsOfTheException(ex, "Lỗi khi kiểm tra giới hạn gửi mã OTP");
            }
        }

        /// <summary>
        /// Hàm điều kiển trước khi gửi mã OTP đến đến client
        /// </summary>
        private async Task BeforeSendingOTP(string Key)
        {
            try
            {
                //Kiểm tra email hoặc người dùng có tồn tại không
                bool findByUserEmail = await _unitOfWork.users.IsUserEmailExists(Key);
                bool findByUserPhoneNume = await _unitOfWork.users.IsUserPhoneNumerExists(Key);
                if ((findByUserEmail || findByUserPhoneNume) == false)
                    throw new ResourceNotFoundException("Không thông tin người dùng tồn tại trong hệ thống");

                //Kiểm tra xem mã OTP dựa trên email có còn hạn không
                bool isKeyExists = await _redisServices.KeyExists(Key);
                if (isKeyExists)
                    throw new Core.Exceptions.InvalidOperationException("Mã OTP đã được gửi đến email này, vui lòng kiểm tra lại email của bạn");

                //Kiểm tra xem có vượt quá giới hạn gửi mã OTP không
                if (await HasExceededRateLimit(Key))
                    throw new TooManyRequestsException("Vượt quá giới hạn gửi mã OTP, vui lòng thử lại sau 1 giờ");
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi chi tiết khi gửi mail {ex.Message} ", ex);
                throw new DetailsOfTheException(ex, "Lỗi chi tiết khi gửi mail");
            }
        }

        /// <summary>
        /// Lấy địa chỉ gửi dựa trên loại dịch vụ
        /// </summary>
        private string GetFromAddress(string serviceType)
        {
            return serviceType switch
            {
                "MailTrap" => _configuration["Authentication:MailTrap:EmailTest"],
                "SMS" => _configuration["Authentication:Vonage:FromNumber"],
                _ => throw new Core.Exceptions.InvalidOperationException("Loại dịch vụ không hợp lệ")
            };
        }

        /// <summary>
        /// Gửi mã OTP
        /// </summary>
        private async Task SendOTPAsync(string recipient, string serviceType, string contentKey)
        {
            try
            {

                //Kiểm tra hợp lệ
                CheckValid(recipient, $"{contentKey} không được bỏ trống");
                await BeforeSendingOTP(recipient);

                //Tạo mã OTP ngẫu nhiên
                string redisKey = contentKey.Equals("phone", StringComparison.OrdinalIgnoreCase)
                    ? $"OTP_Phone:{recipient}"
                    : $"OTP_Email:{recipient}";
                string otp = CodeGenerator.GenerateRandomNumber(6);
                await _redisServices.Set<string>(redisKey, otp, TimeSpan.FromMinutes(5));
                

                //Lấy mẫu nội dung gửi
                string template = contentKey.Equals("phone", StringComparison.OrdinalIgnoreCase)
                    ? VerificationForm.OTPcodeVerificationForm_SMS(otp)
                    : VerificationForm.OTPcodeVerificationForm_Mail(otp);

                //Lấy địa chỉ gửi
                string fromAddress = GetFromAddress(serviceType);

                var messModel = new MessageModel
                {
                    From = fromAddress,
                    To = recipient,
                    Subject = "Mã xác thực OTP",
                    text = template
                };

                //Gửi mã OTP
                await _sendVerificationMessage
                    .GetSendVerificationMessageStrategy(serviceType)
                    .SendVerificationCodeAsync(messModel);

                _logger.Info($"Mã OTP: {otp}, đã được gửi đến {recipient} thành công.");
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi gửi mã OTP: {ex.Message}", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi gửi mã OTP");
            }
        } 
        

        /// <summary>
        /// Gửi mã OTP đến mail - Mailtrap
        /// </summary>
        public async Task AddOTP_EmailToRedis_Mailtrap(string email)
        {
            await SendOTPAsync(email, "MailTrap", "email");
        }

        /// <summary>
        /// Gửi mã OTP đến mail - Mailtrap
        /// </summary>
        public async Task AddOTP_PhoneNumToRedis_Vonage(string phoneNum)
        {
           await SendOTPAsync(phoneNum, "SMS", "phone");
        }

        /// <summary>
        /// Xác thực mã OTP từ email(key)
        /// </summary>
        public async Task<bool> VerifyOTP_Email(VerifyOTP_DTO info)
        {
            try
            {
                //Kiểm tra đầu vào không được bỏ trống
                CheckValid(info.EmailorPhoneNum, "Email");
                CheckValid(info.OTP, "Mã OTP");

                //Kiểm tra email người dùng có tồn tại không
                bool findByUserEmail = await _unitOfWork.users.IsUserEmailExists(info.EmailorPhoneNum);
                if (!findByUserEmail)
                    throw new ResourceNotFoundException("Email không tồn tại trong hệ thống");

                //Kiểm tra email trong redis có tồn tại không
                bool isKeyExists = await _redisServices.KeyExists(info.EmailorPhoneNum);
                if (!isKeyExists)
                    throw new Core.Exceptions.InvalidOperationException("Mã OTP đã hết hạn, vui lòng yêu cầu mã mới");

                //So sánh
                string redisOTP = await _redisServices.Get<string>(info.EmailorPhoneNum);
                if (redisOTP != info.OTP)
                    throw new Core.Exceptions.InvalidOperationException("Mã OTP không chính xác, vui lòng kiểm tra lại mã OTP của bạn");

                //Xóa mã OTP trong redis
                return await _redisServices.Remove(info.EmailorPhoneNum);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi xác thực mail hoặc sms {ex.Message} ", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xác thực mail hoặc sms");
            }
        }
    }
}