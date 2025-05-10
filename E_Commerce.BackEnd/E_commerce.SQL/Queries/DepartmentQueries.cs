namespace E_commerce.SQL.Queries
{
    public static class DepartmentQueries
    {
        //Lấy tất cả
        public static string GetAll => 
            "SELECT * FROM Department;";

        //Lấy theo ID
        public static string FindByID => 
            "SELECT * FROM Department d WHERE d.dep_id = @dep_id;";

        //Thêm
        public static string Add =>
            @"INSERT INTO Department(dep_name,infor)
            VALUES(@dep_name,@infor);";
        
        //Sửa (put)
        public static string UpdateByID =>
            @"UPDATE Department SET  
            	dep_name = @dep_name, 
            	infor = @infor 
            WHERE dep_id = @dep_id;";

        //Xóa
        public static string DeleteByID =>
            "DELETE FROM Department WHERE dep_id = @dep_id;";

        //Sửa (patch)
        public static string PatchByID => 
            @"UPDATE Department SET
            	dep_name = COALESCE(@dep_name,dep_name),
            	infor = COALESCE(@infor,infor)
            WHERE dep_id = @dep_id;";
    }
}