using E_commerce.Application.Application;
using E_commerce.Core.Exceptions;

namespace E_commerce.Infrastructure.Services.impl
{
    public class TokenListManagementService: ITokenListManagementService
    {
        #region ===[Private Member]===
        public readonly IRedisServices _redisService;
        private readonly ILogger _logger;
        #endregion

        #region ===[Constructor]===
        public TokenListManagementService(
            IRedisServices redisService,
            ILogger logger)
        {
            _redisService = redisService 
                ?? throw new ArgumentNullException(nameof(redisService));
            _logger = logger;
        } 
        #endregion
 
        #region ===[public members]===
        /// <summary>
        /// Thêm refresh token vào trong sorted set
        /// </summary>
        public async Task<bool> AddTokenToSortedSet(string token, double score, string key = "white_list"){
            try
            {
                return await _redisService.SortedSetAdd<string>(key, score, token);
            }
            catch(Exception ex){
                _logger.Error($"Lỗi khi thêm phần tử vào trong SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }
        
        /// <summary>
        /// Xóa refresh token trong sorted set
        /// </summary>
        public async Task<bool> DeleteTokenFromSortedSet(string token, string key = "white_list"){
            try
            {
                return await _redisService.SortedSetRemove<string>(key,token); 
            }
            catch(Exception ex){
                _logger.Error($"Lỗi khi xóa phần tử vào trong SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }
        
        /// <summary>
        /// Lấy danh sách refresh token trong sorted set
        /// </summary>
        public async Task<IReadOnlyList<string>> GetAllTokensFromSortedSet(string key = "white_list"){
            try
            {
                var result = await _redisService.SortedSetRangeByRank<string>(key, 0, -1);
                return result.ToList();
            }
            catch(Exception ex){
                _logger.Error($"Lỗi khi lấy danh sách phần tử trong SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }      

        /// <summary>
        /// Xóa token hết hạn
        /// </summary>
        public async Task<bool> DeleteExpiredTokens(string key = "white_list"){
            try{
                /*
                 - Lấy thời điểm hiện tại bằng cách sử dụng DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                 - Xóa các token trong danh sách sorted dựa trên phần score (score này là lưu giá trị hạn sử dụng của token)
                */
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();       //Lấy thời điểm hiện tại
                return await _redisService.SortedSetRemoveScoreByRange(key, double.NegativeInfinity, now);
            }
            catch(Exception ex){
                _logger.Error($"Lỗi khi xóa các token hết hạn trong SortedSet: {ex.Message}", ex);
                return false;
            } 
        }

        /// <summary>
        /// Kiểm tra giá trị trong whitelisst có tồn tại hay không
        /// </summary> 
        public async Task<double> IsTokenInSortedSet(string token, string key = "white_list"){
            try{
                return await _redisService.SortedSetGetScoreByValue(key, token);
            }
            catch(Exception ex) when (ex.Message.Contains("Không tìm thấy giá trị trong SortedSet")){
                _logger.Error($"Token không tồn tại trong whitelist: {ex.Message}", ex);
                throw new DetailsOfTheException(ex,"Không tìm thấy giá trị trong SortedSet");
            }
            catch(Exception ex){
                _logger.Error($"Lỗi khi kiểm tra phần tử trong SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
        #endregion
    }
}