namespace E_commerce.SQL.Queries
{
    public static class UserPhotoDetailsQueries
    {
        /// <summary>
        /// Lấy image_ID dựa trên UID
        /// </summary>
        public static string GetImageIdByUserId =>
            @"SELECT i.img_id, i.public_id
            FROM UserPhotoDetails up
            JOIN Images i ON up.img_id = i.img_id
            WHERE up.user_id = @user_id
            LIMIT 1;";

        /// <summary>
        /// Lấy (image_ID, publicID) dựa trên UID
        /// </summary>
        public static string GetImageId_PublicIdByUserId =>
            @"SELECT i.img_id, i.public_id, i.img_id
            FROM Images i 
            JOIN UserPhotoDetails u ON i.img_id = u.img_id
            WHERE u.user_id = @user_id; ";

        /// <summary>
        /// Thêm 
        /// </summary>
        public static string AddUserPhotoDetails =>
            @"INSERT INTO UserPhotoDetails (user_id, img_id)
            VALUES (@user_id, @img_id);";
    }
}