namespace E_commerce.SQL.Queries
{
    public static class SalesPriceQueries
    {
        //Get All
        public static string GetAll =>
            @"SELECT * FROM SalesPrice;"; 

        //Get By product_id
        public static string GetByProduct_id => 
            @"SELECT * FROM Promotion  
            WHERE product_id = @product_id;";

        //Get By product_id and promotion_id
        public static string GetByProduct_idAndPromotion_id => 
            @"SELECT * FROM Promotion  
            WHERE promo_id = @promo_id AND product_id = @product_id;";

        //Get By promotion_id
        public static string GetByPromotion_id => 
            @"SELECT * FROM Promotion  
            WHERE promo_id = @promo_id ;";

        //Delete By product_id
        public static string Delete =>
            @"DELETE FROM SalesPrice
            WHERE product_id=@product_id;"; 
        
        //Add 
        public static string Add => 
            @"INSERT INTO SalesPrice(price,num_of_product,_time,product_id,promo_id)
            VALUES(@price,@num_of_product,@_time,@product_id,@promo_id);";
        
        //Update
        //public static string UpdateBy_=> 
        //public static string => 
    }
}