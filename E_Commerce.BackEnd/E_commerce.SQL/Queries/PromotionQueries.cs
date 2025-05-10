namespace E_commerce.SQL.Queries
{
    public static class PromotionQueries
    {
        public static string GetALL => @"SELECT * FROM Promotion";

        public static string FindByID =>
            @"SELECT * FROM Promotion  
            WHERE promo_id = @promo_id;";

        public static string Add =>
            @"INSERT INTO Promotion(promo_name,discount,start_time,end_time)
            VALUES(@promo_name,@discount,@start_time,@end_time);";

        public static string Update_PUT =>
            @"UPDATE Promotion
            SET promo_name = @promo_name, 
            	discount=@discount,
            	end_time=@end_time,
            	start_time=@start_time
            WHERE promo_id = @promo_id;";

        public static string Update_PATCH =>
            @"UPDATE Promotion 
            SET `promo_name` = COALESCE(@promo_name, `promo_name`), 
            	discount = COALESCE(@discount, discount),
            	start_time = COALESCE(@start_time, start_time),
            	end_time = COALESCE(@end_time, end_time),
            WHERE promo_id = @promo_id;";

        public static string DeleteByID =>
            @"DELETE FROM Promotion
            WHERE promo_id=@promo_id; ";
    }
}