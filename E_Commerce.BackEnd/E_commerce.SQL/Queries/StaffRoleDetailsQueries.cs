namespace E_commerce.SQL.Queries
{
    public static class StaffRoleDetailsQueries
    {
        //Lấy tất cả
        public static string GetAll =>
            @"SELECT * FROM StaffRoleDetails;";
        
        //Lấy theo ID nhân viên
        public static string GetByStaffID = @"
            SELECT * FROM StaffRoleDetails 
            WHERE user_emp = @user_emp;";

        // Thêm vai trò cho nhân viên
        public static string AddRoleForStaff = @"
            INSERT INTO StaffRoleDetails(user_emp,`describe`,role_id)
            VALUES(@user_emp,@describe,@role_id);";

        // Xóa chi tiết vai trò theo ID nhân viên
        public static string DeleteRoleForStaff = @"
            DELETE FROM StaffRoleDetails 
            WHERE user_emp = @user_emp AND role_id = @role_id;";

        //Xóa hết vai trò của nhân viên
        public static string DeleteAllRoleForStaff = @"
            DELETE FROM StaffRoleDetails 
            WHERE user_emp = @user_emp;";
    
        // Cập nhật chi tiết vai trò 
        public static string UpdateRoleForStaff = @"
            UPDATE StaffRoleDetails 
            SET `describe` = @describe, role_id = @newRole_id, user_emp = @user_emp
            WHERE user_emp = @user_emp AND role_id = @role_id;";
        
        //Cập nhật (PATCH)
        public static string UpdateRoleForStaffPatch = @"
            UPDATE StaffRoleDetails 
            SET `describe` = COALESCE(@describe, `describe`), 
            	role_id = COALESCE(@newRole_id, role_id)
            WHERE user_emp = @user_emp AND role_id = @role_id;";

        // Lấy thông tin các nhân viên theo ID vai trò
        public static string GetStaffByRoleID = @"
        SELECT u.user_id, u.user_name, u.date_of_birth, u.address,
            u.phone_num, u.email, u.is_block, u.is_delete
        FROM `User` u
        JOIN StaffRoleDetails sr ON u.user_id = sr.user_emp
        WHERE sr.role_id = @role_id;";

        // Lấy thông tin vai trò dựa trên ID nhân viên
        public static string GetRoleByStaffID = @"
            SELECT r.role_id, r.role_name, r.role_name
            FROM `Role` r
            JOIN StaffRoleDetails sr ON r.role_id = sr.role_id
            WHERE sr.user_emp = @user_emp;";
    }
}