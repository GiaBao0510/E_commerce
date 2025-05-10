
namespace E_commerce.SQL.Queries
{
    public static class ProductTypeQueries
    {
        #region ====[CRUD]===

        public static string Add =>
            @"INSERT INTO ProductType(protyle_name,alias_name,details)
            VALUES(@protyle_name,@alias_name,@details);";
        
        public static string findByID =>
            @"SELECT * FROM ProductType 
            WHERE protyle_id=@protyle_id;";
        
        public static string DeleteByID =>
            @"DELETE FROM ProductType
            WHERE protyle_id=@protyle_id; ";

        public static string UpdateByID_PUT =>
            @"UPDATE ProductType
            SET protyle_name = @protyle_name, 
            	alias_name=@alias_name,
            	details=@details
            WHERE protyle_id = @protyle_id;";


        public static string UpdateByID_PATCH =>
            @"UPDATE ProductType
            SET protyle_name = COALESCE(@protyle_name, protyle_name),
            	alias_name= COALESCE(@alias_name,alias_name),
            	details= COALESCE(@details, details)
            WHERE protyle_id = @protyle_id;";

        public static string GetAll =>
            @"SELECT * FROM ProductType;";
        #endregion
    }
}