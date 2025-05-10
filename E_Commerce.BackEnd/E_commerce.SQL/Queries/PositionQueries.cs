namespace E_commerce.SQL.Queries
{
    public static class PositionQueries
    {
        //Lấy tất cả
        public static string GetAll => 
            "SELECT * FROM PositionStaff;";

        //Lấy theo ID
        public static string FindByID => 
            "SELECT * FROM PositionStaff ps WHERE ps.position_id = @position_id;";

        //Thêm
        public static string Add =>
            @"INSERT INTO PositionStaff(position_name,allowance_coefficient)
            VALUES(@position_name,@allowance_coefficient);";
        
        //Sửa (put)
        public static string UpdateByID =>
            @"UPDATE PositionStaff SET 
            	position_name = @position_name,
            	allowance_coefficient = @allowance_coefficient
            WHERE position_id = @position_id;";

        //Xóa
        public static string DeleteByID =>
            "DELETE FROM PositionStaff WHERE position_id = @position_id;";

        //Sửa (patch)
        public static string PatchByID => 
            @"UPDATE PositionStaff SET 
            	position_name = COALESCE(@position_name,position_name),
            	allowance_coefficient = COALESCE(@allowance_coefficient,allowance_coefficient)
            WHERE position_id = @position_id;";
    }
}