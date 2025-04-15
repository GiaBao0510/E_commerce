namespace E_commerce.SQL.Queries
{
    public class CustomerQueries
    { 
        public static string AllCustomer =>
            "SELECT u.user_id, u.user_name, u.date_of_birth," +
                "u.address, u.phone_num, u.email, u.is_block, u.is_delete," +
                "r.rank_id, r.rank_name, r.rating_point " +
            "FROM Customer cs " +
                "JOIN `User` u ON cs.user_client = u.user_id " + 
                "JOIN CustomerRoleDetails cdt ON cdt.user_client = cs.user_client " +
                "JOIN `Rank` r ON r.rank_id = cdt.rank_id;";

        public static string CustomerByID =>
            "SELECT u.user_id, u.user_name, u.date_of_birth," +
                "u.address, u.phone_num, u.email, u.is_block, u.is_delete, " +
                "r.rank_id, r.rank_name, r.rating_point " +
            "FROM Customer cs " +
                "JOIN `User` u ON cs.user_client = u.user_id " +
                "JOIN CustomerRoleDetails cdt ON cdt.user_client = cs.user_client " +
                "JOIN `Rank` r ON r.rank_id = cdt.rank_id " +
            "WHERE cs.user_client = @user_client;";

        public static string AddCustomer => 
            "INSERT INTO Customer(user_client) VALUES(@user_client);";

        public static string AddBasicCustomerInformation => 
            "INSERT INTO CustomerRoleDetails(user_client,rank_id,role_id) " +
                "VALUES(@user_client, 7, 6);";

        public static string UpdateCustomerRank =>
            "UPDATE CustomerRoleDetails(user_client,rank_id) " +
                "VALUES(@user_client, @rank_id); ";

        public static string PatchCustomer => @"
            UPDATE `User` u 
            INNER JOIN CustomerRoleDetails csd ON u.user_id = csd.user_client 
            SET u.user_name = COALESCE(@user_name, user_name),
            	u.date_of_birth = COALESCE(@date_of_birth, date_of_birth),
            	u.address = COALESCE(@address, address), 
            	u.phone_num = COALESCE(@phone_num, phone_num), 
            	u.email = COALESCE(@email, email),
            	csd.rank_id = COALESCE(@rank_id, rank_id), 
            	csd.role_id = COALESCE(@role_id, role_id) 
            WHERE u.user_id = @user_id;";
    }
}