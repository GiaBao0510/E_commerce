using E_commerce.Application.DTOs.Common;
using E_commerce.Application.DTOs.Requests;

namespace E_commerce.Infrastructure.Services
{
    public interface IAccountServices
    {
        //Kiểm tra tài khoản có bị khóa không
        public Task<bool> CheckIfAccountIsBlocked(string phone_num);

        //Kiểm tra tài khoản có bị xóa không
        public Task<bool> CheckIfAccountIsDeleted(string phone_num);

        //Đăng nhập tài khoản
        public Task<(TokenDTO, TokenDTO)> Login(LoginDTO login);
        
        //Đăng ký
        //Đăng xuất
        //Lấy thông tin tài khoản
        
        // Xóa tài khoản
        public Task<bool> DeleteAccount(string user_id);

        //Khóa tài khoản
        public Task<bool> BlockAccount(string user_id);

        //Khôi phục tài khoản
        public Task<bool> RecoverAccount(string user_id);

        //Mở khóa tài khoản
        public Task<bool> UnlockAccount(string user_id);

        //Thay đổi mật khẩu
        public Task<bool> ChangePassword(string user_id, ChangePasswordDTO changePasswordDTO);
    }
}