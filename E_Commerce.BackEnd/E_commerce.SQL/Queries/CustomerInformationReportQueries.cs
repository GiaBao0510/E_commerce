
namespace E_commerce.SQL.Queries
{
    public static class CustomerInformationReportQueries
    {
        //Báo cáo danh sách người tiêu dùng bị khóa
        public static string ReportBlockedCustomerList =>
            "SELECT u.user_id, u.user_name, u.date_of_birth," +
	            "u.address, u.phone_num, u.email " + 
            "FROM Customer cs "+
            "JOIN `User` u ON cs.user_client = u.user_id "+
            "WHERE u.is_block = 1";

        //Báo cáo danh sách người tiêu dùng bị xóa
        public static string ReportDeletedCustomerList => 
            "SELECT u.user_id, u.user_name, u.date_of_birth," +
	            "u.address, u.phone_num, u.email " + 
            "FROM Customer cs "+
            "JOIN `User` u ON cs.user_client = u.user_id "+
            "WHERE u.is_delete = 1";

        //Báo cáo danh sách người tiêu dùng theo thứ hạng 
        public static string ReportCustomerListByRank =>
            "SELECT u.user_id, u.user_name, u.date_of_birth," +
	            "u.address, u.phone_num, u.email " + 
            "FROM Customer cs "+
            "JOIN `User` u ON cs.user_client = u.user_id "+
            "JOIN CustomerRoleDetails cdt ON cdt.user_client = cs.user_client "+
            "JOIN `Rank` r ON r.rank_id = cdt.rank_id "+
            "WHERE r.rank_id = @rank_id";
    }
}