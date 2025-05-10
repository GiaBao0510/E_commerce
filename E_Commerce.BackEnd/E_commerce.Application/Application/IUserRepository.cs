using E_commerce.Application.DTOs.Common;
using E_commerce.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace E_commerce.Application.Application 
{
    public interface IUserRepository : IRepository<_User>
    {
        public void ValidateUser(_User user);
        public void ValidateUserId(string id);
        public Task<_User> IsUserIdExists(string id);

        /// <summary>
        /// Kiểm tra xem email của người dùng có tồn tại hay không
        /// </summary>
        public Task<bool> IsUserEmailExists(string email);

        /// <summary>
        /// Kiểm tra xem số điện thoại của người dùng có tồn tại hay không
        /// </summary>
        public Task<bool> IsUserPhoneNumerExists(string phone_num);
        
        /// <summary>
        /// Lấy mật khẩu đã băm dựa trên ID người dùng
        /// </summary>
        public Task<string> GetHashedPasswordByUserID(string uid);
        
        /// <summary>
        /// Lấy vai trò dựa trên ID người dùng
        /// </summary>
        public Task<IReadOnlyList<RoleNamesDTO>> ListOfRoleNames(string uid);

        /// <summary>
        /// Lấy thông tin cơ bản người dùng
        /// </summary>
        public Task<BasicUserInfoDTO> GetBasicUserInfo(string uid);

        /// <summary>
        /// Thêm ảnh ngươi dùng dựa trên ID người dùng
        /// </summary>
        public Task AddImageForUser(string uid, IFormFile file);

        /// <summary>
        /// Xóa ảnh người dùng dựa trên ID người dùng
        /// </summary>
        public Task DeleteImageForUser(string uid);

        /// <summary>
        /// Câp nhật ảnh người dùng dựa trên ID người dùng
        /// </summary>
        public Task UpdateImageForUser(string uid, IFormFile file);
    }
}