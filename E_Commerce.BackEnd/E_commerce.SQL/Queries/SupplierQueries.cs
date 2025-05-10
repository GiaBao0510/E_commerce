namespace E_commerce.SQL.Queries
{
    public static class SupplierQueries
    {
        #region ====["CRUD"]====
        public static string Insert =>
            @"INSERT INTO Supplier(sup_name,email,phone_num,address,contact_person,detail,tax_code)
            VALUES(@sup_name,@email,@phone_num,@address,@contact_person,@detail,@tax_code);";
        
        public static string DeleteByID =>
            @"DELETE FROM Supplier
            WHERE sup_id=@sup_id;";

        public static string GetAll =>
            @"SELECT * FROM Supplier;";
        public static string UpdateByID_PUT=>
            @"UPDATE Supplier
            SET sup_name = @sup_name, 
            	phone_num=@phone_num,
            	address=@address,
                email=@email, 
            	contact_person=@contact_person,
            	detail=@detail, 
            	tax_code=@tax_code
            WHERE sup_id = @sup_id;";

        public static string UpdateByID_PATCH=>
            @"UPDATE Supplier
            SET sup_name = COALESCE(@sup_name, sup_name), 
            	phone_num= COALESCE(@phone_num, phone_num),
            	address= COALESCE(@address, address), 
                email = COALESCE(@email, email), 
            	contact_person= COALESCE(@contact_person, contact_person),
            	detail= COALESCE(@detail, detail)
            	tax_code= COALESCE(@tax_code,tax_code)
            WHERE sup_id = @sup_id;";

        public static string GetByID =>
            @"SELECT * FROM Supplier WHERE sup_id=@sup_id;";
        #endregion
    }
}