using E_commerce.Application.DTOs.Common;
using E_commerce.Core.Entities;

namespace E_commerce.Application.Application 
{
    public interface IUserRepository : IRepository<_User>
    {
        public void ValidateUser(_User user);
        public void ValidateUserId(string id);
        public Task<_User> IsUserIdExists(string id);
        
        /// <summary>
        /// Lấy mật khẩu đã băm dựa trên ID người dùng
        /// </summary>
        public Task<string> GetHashedPasswordByUserID(string uid);
        
        /// <summary>
        /// Lấy vai trò dựa trên ID người dùng
        /// </summary>
        public Task<IReadOnlyList<RoleNamesDTO>> ListOfRoleNames(string uid);
    }
}