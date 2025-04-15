using E_commerce.Application.Application;
using E_commerce.Core.Exceptions;

namespace E_commerce.Infrastructure.Services.impl
{
    public class TokenListService: ITokenListService
    {
        #region ===[Private Member]===
        public readonly IRedisServices _redisService;
        private readonly ILogger _logger;
        #endregion

        #region ===[Constructor]===
        public TokenListService(
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
        /// Thêm refresh token vào trong sorted set, đông thời ánh xạ UID với token
        /// </summary>
        public async Task<bool> AddTokenToSortedSetWithMapping(string UID, string token, double score, string key = "white_list"){
            try
            {
                //Lưu ánh xạ user_id -> token vào trong redis
                await _redisService.Set<string>($"user_id:{UID}", token);

                //Lưu ánh xạ ngược
                await _redisService.Set<string>($"token:{token}", UID);

                //Thêm token vào trong sorted set
                return await _redisService.SortedSetAdd<string>(key, score, token);
            }
            catch(Exception ex){
                _logger.Error($"Lỗi khi thêm phần tử vào trong SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }
        
        /// <summary>
        /// Lấy refreshtokem dựa trên UID 
        /// </summary>
        public async Task<string> GetTokenByUID(string UID){
            try
            {
                return await _redisService.Get<string>($"user_id:{UID}");
            }
            catch(Exception ex){
                _logger.Error($"Lỗi khi lấy RefreshToken theo user_id {UID}: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }

        /// <summary>
        /// Lấy UID dựa trên refreshtoken 
        /// </summary>
        public async Task<string> GetUserIDByToken(string token){
            try
            {
                return await _redisService.Get<string>($"token:{token}");
            }
            catch(Exception ex){
                _logger.Error($"Lỗi khi lấy UID theo RefreshToken {token}: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Xóa refreshtokem dựa trên UID 
        /// </summary>
        public async Task RemoveToken_UserID(string UID, string token){
            try
            {
                await _redisService.Remove($"user_id:{UID}");
                await _redisService.Remove($"token:{token}");
            }
            catch(Exception ex){
                _logger.Error($"Lỗi khi xóa ánh xạ RefreshToken cho user_id và ngược lại {UID}: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }

        
        /// <summary>
        /// Xóa refresh token trong sorted set
        /// </summary>
        public async Task DeleteTokenFromSortedSet(string token, string key = "white_list"){
            try
            {
                await _redisService.SortedSetRemove<string>(key,token);
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
        public async Task DeleteExpiredTokens(string key = "white_list"){
            try{
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();                                                //Lấy thời điểm hiện tại
                var expiredTokens = await _redisService.SortedSetRangeByScore<string>(key, double.NegativeInfinity, now);    //Lấy danh sách các tokens hết hạn theo phạm vi
                await _redisService.SortedSetRemoveScoreByRange(key, double.NegativeInfinity, now);
            
                //Xóa ánh xạ cho các token đã hết hạn
                foreach(var token in expiredTokens){
                    
                    //Lấy UID dựa trên token
                    var UID = await _redisService.Get<string>($"token:{token}");
                    if(!string.IsNullOrEmpty(UID)){
                        //Xóa ánh xạ token -> UID
                        await _redisService.Remove($"token:{token}");
                        //Xóa ánh xạ UID -> token
                        await _redisService.Remove($"user_id:{UID}");
                    }
                }
            }
            catch(Exception ex){
                _logger.Error($"Lỗi khi xóa các token hết hạn trong SortedSet: {ex.Message}", ex);
            } 
        }

        /// <summary>
        /// Kiểm tra giá trị trong whitelisst có tồn tại hay không
        /// </summary>
        public async Task<double> IsTokenInSortedSet(string token, string key = "white_list"){
            try{
                return await _redisService.SortedSetGetScoreByValue(key, token);
            }
            catch(Exception ex){
                _logger.Error($"Lỗi khi kiểm tra phần tử trong SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
        #endregion
    }
}