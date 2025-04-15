namespace E_commerce.SQL.Queries
{
    public class AccountQueries
    {
        //Kiểm tra tài khoản có bị khóa không
        public static string CheckIfAccountIsBlocked =>
            "SELECT Count(phone_num) FROM `User` WHERE phone_num = @phone_num AND is_block = 1;";

        //Kiểm tra tài khoản có bị xóa không
        public static string CheckIfAccountIsDeleted =>
            "SELECT Count(phone_num) FROM `User` WHERE phone_num = @phone_num AND is_delete = 1;";

        
        //Lấy (mật khẩu đã băm, tài khoản, uid) dựa trên tài khoản
        public static string CheckIfAccountExists =>
            "SELECT pass_word, phone_num, user_id, email, user_name FROM `User` WHERE phone_num = @phone_num;";
        
        //Xóa tài khoản
        public static string DeleteAccount =>
            "UPDATE `User` SET is_delete = 1 WHERE user_id = @user_id;";
        
        //Khóa tài khoản
        public static string BlockAccount =>
            "UPDATE `User` SET is_block = 1 WHERE user_id = @user_id;";
        
        //Mở khóa tài khoản
        public static string UnlockAccount =>
            "UPDATE `User` SET is_block = 0 WHERE user_id = @user_id;";
        
        //Khôi phục tài khoản
        public static string RecoverAccount =>
            "UPDATE `User` SET is_delete = 0 WHERE user_id = @user_id;";
        
        //Thay đổi mật khẩu
        public static string ChangePassword =>
            "UPDATE `User` SET pass_word = @new_pass_word WHERE user_id = @user_id;";
    }
}