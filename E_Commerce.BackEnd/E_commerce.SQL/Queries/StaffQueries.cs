namespace E_commerce.SQL.Queries
{
    public static class StaffQueries
    {
        public static string GetAll =>
            @"SELECT s.user_emp, u.user_name, u.date_of_birth, u.address, u.phone_num, 
            	u.email, u.is_block, u.is_delete, s.account_number
            FROM Staff s
            JOIN `User` u ON s.user_emp = u.user_id";
        
        public static string GetByID =>
            @"SELECT s.user_emp, u.user_name, u.date_of_birth, u.address, u.phone_num, 
            	u.email, u.is_block, u.is_delete
            FROM Staff s
            JOIN `User` u ON s.user_emp = u.user_id
            WHERE s.user_emp = @user_emp;";
        
        public static string IsStaff =>
            @"SELECT COUNT(*) FROM Staff WHERE user_emp = @user_emp;";

        public static string Add => 
            @"
            INSERT INTO Staff(user_emp, account_number)
            VALUES(@user_emp, @account_number);
            ";
        
        public static string UpdateByID =>
            @"
            UPDATE `User` u
            INNER JOIN Staff s ON u.user_id = s.user_emp
            SET u.user_name = COALESCE(@user_name, u.user_name),
            	u.date_of_birth = COALESCE(@date_of_birth, u.date_of_birth),
            	u.address = COALESCE(@address, u.address),
            	u.phone_num = COALESCE(@phone_num, u.phone_num),
            	u.email = COALESCE(@email, u.email),
            	s.account_number = COALESCE(@account_number, s.account_number)
            WHERE s.user_emp = @user_emp;
            ";
        
        public static string PatchStaff => @"
            UPDATE `User` u
            INNER JOIN Staff s ON u.user_id = s.user_emp
            SET u.user_name = COALESCE(@user_name, u.user_name),
            	u.date_of_birth = COALESCE(@date_of_birth, u.date_of_birth),
            	u.address = COALESCE(@address, u.address),
            	u.phone_num = COALESCE(@phone_num, u.phone_num),
            	u.email = COALESCE(@email, u.email),
            	s.account_number = COALESCE(@account_number, s.account_number)
            WHERE s.user_emp = @user_emp;
        ";
    }
}