namespace E_commerce.SQL.Queries
{
    public static class FileDataProccessingQueries
    {
        /// <summary>
        /// Kiểm tra bảng có tồn tại không
        /// </summary>
        public static string CheckTableExist =>
            @"SELECT COUNT(*) FROM information_schema.tables
            WHERE table_schema = DATABASE() AND TABLE_NAME = @TABLE_NAME;";

        /// <summary>
        /// Thiết lập tắt kiểm tra điều kiện trong phiên làm việc
        /// </summary>
        public static string TurnOffSessionInformationChecking =>
            "SET SESSION sql_log_bin = 0; " +           //Tắt ghi log nhị phân cho phiên hiện tại
            "SET SESSION foreign_key_checks = 0;" +     //Tắt kiểm tra ràng buộc khóa ngoại cho phiên hiện tại
            "SET SESSION unique_checks = 0;" +          //Tắt kiểm tra ràng buộc duy nhất cho phiên hiện tại
            "SET SESSION autocommit = 1;";              //Bật chế độ commit sau mỗi lệnh SQL
    }
}