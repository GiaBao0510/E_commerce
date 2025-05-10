namespace E_commerce.SQL.Queries
{
    public static class RoleQueries
    {
        public static string AllRole => "SELECT * FROM Role"; 
        
        public static string RoleByID => "SELECT * FROM Role WHERE role_id = @role_id";
        
        public static string AddRole =>
            @"INSERT INTO Role(role_name, `describe`) VALUES(@role_name, @describe);";
        
        public static string UpdateRole =>
            @"UPDATE Role SET role_name = @role_name, `describe` = @describe WHERE role_id = @role_id";
        
        public static string DeleteRole => 
            "DELETE FROM Role WHERE role_id = @role_id";
        public static string PatchRoleName 
            => "UPDATE Role SET role_name = @role_name WHERE role_id = @role_id";
        public static string PatchRoleDescription
            => "UPDATE Role SET `describe` = @describe WHERE role_id = @role_id"; 
    }
}