using E_commerce.Application.Application;
using E_commerce.Core.Exceptions;
using E_commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Infrastructure.Utils
{
    public class CheckoForDuplicateErrors: ICheckoForDuplicateErrors
    {
        #region ===[Private Member]===
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        #endregion

        public CheckoForDuplicateErrors(ILogger logger, ApplicationDbContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Kiểm tra xem Số điện thoại đã tồn tại trong hệ thống hay chưa
        /// </summary>
        public async Task CheckForDuplicatePhonenumbers(string phonenum){
            var result = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.PhoneNum == phonenum);
            
            if(result != null)
            {
                _logger.Error($"Số điện thoại {phonenum} đã tồn tại trong hệ thống.");
                throw new ResourceConflictException($"Số điện thoại {phonenum} đã tồn tại trong hệ thống.");
            }
        }

        /// <summary>
        /// Kiểm tra xem Email đã tồn tại trong hệ thống hay chưa
        /// </summary>
        public async Task CheckForDuplicateEmails(string email){
            var result = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Email == email);
            
            if(result != null)
            {
                _logger.Error($"Email {email} đã tồn tại trong hệ thống.");
                throw new ResourceConflictException($"Email {email} đã tồn tại trong hệ thống.");
            }
        }

        /// <summary>
        /// Kiểm tra xem UID đã tồn tại trong hệ thống hay chưa
        /// </summary>
        public async Task CheckForDuplicateUIDs(string uid){
            var result = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.UserId == uid);
            
            if(result != null)
            {
                _logger.Error($"UID {uid} đã tồn tại trong hệ thống.");
                throw new ResourceConflictException($"UID {uid} đã tồn tại trong hệ thống.");
            }
        }

        /// <summary>
        /// Kiểm tra trùng lặp email khi cập nhật
        /// </summary>
        public async Task CheckForDuplicateWhenUpdatingEmail(string email){
            
            var count = await _dbContext.Users
                .CountAsync(x => x.Email == email && x.Email != null);
            
            if(count > 1){
                _logger.Error($"Đã trùng lặp Email {email} .Trong khi cập nhật.");
                throw new ResourceConflictException($"Đã trùng lặp Email {email} .Trong khi cập nhật.");
            }
        }

        /// <summary>
        /// Kiểm tra trùng lặp phone_number khi cập nhật
        /// </summary>
        public async Task CheckForDuplicateWhenUpdatingPhoneNumber(string phoneNumber){
            
            var count = await _dbContext.Users
                .CountAsync(x => x.PhoneNum == phoneNumber && x.PhoneNum != null);
            
            if(count > 1){
                _logger.Error($"Đã trùng lặp Phone Number {phoneNumber} .Trong khi cập nhật.");
                throw new ResourceConflictException($"Đã trùng lặp Phone Number {phoneNumber} .Trong khi cập nhật.");
            }
        }
            

    }
}