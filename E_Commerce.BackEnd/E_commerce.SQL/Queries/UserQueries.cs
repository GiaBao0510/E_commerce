namespace E_commerce.SQL.Queries
{
    public static class UserQueries
    {
        public static string AllUser => "SELECT * FROM `User` FORCE INDEX (PRIMARY)";
        
        public static string UserByID => "SELECT * FROM `User` WHERE user_id = @user_id";
        
        public static string AddUser => 
            "INSERT INTO `User`(user_id, user_name, date_of_birth, address, phone_num, email, pass_word)"+
            "VALUES(@user_id, @user_name, @date_of_birth, @address, @phone_num, @email, @pass_word)";

        public static string UpdateUser =>
            "UPDATE `User` SET user_name=@user_name, date_of_birth=@date_of_birth, address=@address, phone_num=@phone_num, email=@email "+
            "WHERE user_id=@user_id;";
        
        public static string DeleteUser =>
            "DELETE FROM `User` WHERE user_id = @user_id;";

        public static string PatchUser =>
            @"Update `User` 
            SET 
            user_name = COALESCE(@user_name, user_name),
            date_of_birth = COALESCE(@date_of_birth, date_of_birth),
            address = COALESCE(@address, address),
            phone_num = COALESCE(@phone_num, phone_num),
            email = COALESCE(@email, email) 
            Where user_id = @user_id;";

        public static string CheckIfUserExists =>
            "SELECT * FROM `USER` WHERE user_id = @user_id;";
        
        public static string GetListOfRoleBasedOnUser =>
            @"SELECT r.role_name 
            FROM Customer c 
            JOIN CustomerRoleDetails cd ON c.user_client = cd.user_client 
            JOIN `Role` r ON r.role_id = cd.role_id 
            WHERE c.user_client = @user_id 
            
            UNION ALL 
            
            SELECT r.role_name 
            FROM Staff s 
            JOIN StaffRoleDetails sd ON s.user_emp = sd.user_emp 
            JOIN `Role` r ON r.role_id = sd.role_id 
            WHERE s.user_emp = @user_id;";

        //Lấy mật khẩu đã bắm dựa trên UserID
        public static string GetHashedPasswordByUserID =>
            "SELECT pass_word FROM `User` WHERE user_id = @user_id;";

        //Tìm thông tin người dùng thông qua email
        public static string FindUserByEmail =>
            "SELECT * FROM `User` WHERE email = @email;";
        
        //Tìm kiếm thông tin người dùng thông qua phone_num
        public static string FindUserByPhoneNum =>
            "SELECT * FROM `User` WHERE phone_num = @phone_num;";

        //Tạo người dùng hoặc thêm người dùng mới dựa trên truy xuất email từ người dùng
        public static string GetOrCreateUserByEmail => 
            @"START TRANSACTION;
            INSERT INTO `User` (user_id, user_name, date_of_birth, address, phone_num, email, pass_word, is_block, is_delete)
            SELECT UUID_SHORT(), @user_name, NULL, NULL, NULL, @email, NULL, 0, 0
            WHERE NOT EXISTS (
               SELECT 1 FROM `User` WHERE email = @email
            );
            SELECT * FROM `User` WHERE email = @email;
            COMMIT;";

        //Kiểm tra email người dùng có tồn tại hay không
        public static string FindByUserEmail =>
            "SELECT * FROM `User` WHERE email = @email;";

        public static string GetBasicUserInfo =>
            @"SELECT u.user_id, u.user_name, u.email, u.phone_num
            FROM `User` u 
            WHERE u.user_id = @user_id;";
    }
}