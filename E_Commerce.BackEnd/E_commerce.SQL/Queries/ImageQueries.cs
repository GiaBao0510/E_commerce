namespace E_commerce.SQL.Queries
{
    public static class ImageQueries
    {
        /// <summary>
        /// Xóa ảnh dựa trên ID ảnh
        /// </summary>
        public static string DeleteImageByID =>
            "DELETE FROM Images WHERE img_id = @img_id;";

        /// <summary>
        /// Xóa ảnh dựa trên public_ID ảnh
        /// </summary>
        public static string DeleteImageByPublic_id =>
            "DELETE FROM Images WHERE public_id = @public_id;";
        
        /// <summary>
        /// Lấy ảnh dựa trên ID ảnh
        /// </summary>
        public static string GetImageByID =>
            "SELECT * FROM Images WHERE img_id = @img_id;";
        
        /// <summary>
        /// Cập nhật ảnh dựa trên ID ảnh
        /// </summary>
        public static string UpdateImageByID =>
            @"UPDATE Images
            SET public_id = @public_id,
            	path_img = @path_img
            WHERE img_id = img_id@; ";
        
        /// <summary>
        /// Thêm ảnh
        /// </summary>
        public static string AddImage => 
            @"INSERT INTO Images (public_id, path_img)
            VALUES (@public_id, @path_img);
            SELECT LAST_INSERT_ID();";
    }
}