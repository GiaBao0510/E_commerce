namespace E_commerce.Infrastructure.Services
{
    public interface ITokenListManagementService
    {
        /// <summary>
        /// Thêm access và refresh token vào trong sorted set
        /// </summary>
        public Task<bool> AddTokenToSortedSet(string token, double score, string key = "white_list");

        /// <summary>
        /// Xóa token trong sorted set
        /// </summary>
        public Task<bool> DeleteTokenFromSortedSet(string token, string key = "white_list");
        
        /// <summary>
        /// Lấy danh sách token trong sorted set
        /// </summary>
        public Task<IReadOnlyList<string>> GetAllTokensFromSortedSet(string key = "white_list");
        
        /// <summary>
        /// Xóa token hết hạn 
        /// </summary>
        public Task<bool> DeleteExpiredTokens(string key = "white_list");

        /// <summary>
        /// Kiểm tra giá trị trong whitelist có tồn tại hay không
        /// </summary>
        public Task<double> IsTokenInSortedSet(string token, string key = "white_list");

       
    }
}