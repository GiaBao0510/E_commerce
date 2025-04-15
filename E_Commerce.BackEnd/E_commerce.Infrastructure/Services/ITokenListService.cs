namespace E_commerce.Infrastructure.Services
{
    public interface ITokenListService
    {
        /// <summary>
        /// Thêm refresh token vào trong sorted set
        /// </summary>
        public Task<bool> AddTokenToSortedSet(string token, double score, string key = "white_list");
        
        /// <summary>
        /// Thêm refresh token vào trong sorted set, đông thời ánh xạ UID với token
        /// </summary>
        public Task<bool> AddTokenToSortedSetWithMapping(string UID, string token, double score, string key = "white_list");
        
        /// <summary>
        /// Lấy refreshtoken dựa trên UID 
        /// </summary>
        public Task<string> GetTokenByUID(string UID);

        /// <summary>
        /// Lấy UID dựa trên refreshtoken 
        /// </summary>
        public Task<string> GetUserIDByToken(string UID);

        /// <summary>
        /// Xóa refreshtokem dựa trên UID và ngược lại
        /// </summary>
        public Task RemoveToken_UserID(string UID, string token);

        /// <summary>
        /// Xóa refresh token trong sorted set
        /// </summary>
        public Task DeleteTokenFromSortedSet(string token, string key = "white_list");
        
        /// <summary>
        /// Lấy danh sách refresh token trong sorted set
        /// </summary>
        public Task<IReadOnlyList<string>> GetAllTokensFromSortedSet(string key = "white_list");
        
        /// <summary>
        /// Xóa token hết hạn
        /// </summary>
        public Task DeleteExpiredTokens(string key = "white_list");

        /// <summary>
        /// Kiểm tra giá trị trong whitelist có tồn tại hay không
        /// </summary>
        public Task<double> IsTokenInSortedSet(string token, string key = "white_list");
    }
}