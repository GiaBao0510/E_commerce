using System.Text.Json;
using E_commerce.Application.Application;
using E_commerce.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace E_commerce.Infrastructure.Services.impl
{
    public class RedisServices: IRedisServices
    {
        #region ====[Private Fields]====
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private IConfiguration _configuration{get; }
        private readonly ILogger _logger;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public RedisServices(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;

            try{
                var RedisHost = configuration["Database:Redis:Host"];
                var redisPassword = configuration["Database:Redis:Password"];

                _logger.Info($"Đang kết nối với Redis tại: {RedisHost}");

                //Thiết lập cấu hình
                var option = new ConfigurationOptions{
                    AbortOnConnectFail = false,
                    ConnectTimeout = 5000,
                    SyncTimeout = 5000,
                    ConnectRetry = 3,
                    KeepAlive = 60,
                    ReconnectRetryPolicy = new LinearRetry(1000),
                    AllowAdmin = true,
                    Ssl = false         // Tắt SSL nếu bạn đang kiểm thử cục bộ
                };

                //Thêm end point
                option.EndPoints.Add(RedisHost); // Cổng mặc định của Redis là 6379

                //Thêm password
                option.Password = redisPassword;

                 _redis = ConnectionMultiplexer.Connect(option);
                 _db = _redis.GetDatabase();
            }
            catch(Exception ex){
                _logger.Error($"Lỗi khi khởi tạo kết nối Redis: {ex.Message}", ex);
                throw;
            }
        }

        #region ====[Common ingredient]====
        public async Task<bool> Set<T>(string key, T value){
            try{
                
                // Nếu không kết nối được thì thực hiện kết nối lại
                if(! _redis.IsConnected)    
                    await _redis.GetDatabase().PingAsync();

                var serializedValue = JsonSerializer.Serialize(value);
                return await _db.StringSetAsync(key, serializedValue);
            }catch(Exception ex){
                _logger.Error($"Lỗi khi đặt giá trị tại bộ nhớ đệm: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> Set<T>(string key, T value, TimeSpan? expiration = null){
            try{
                
                // Nếu không kết nối được thì thực hiện kết nối lại
                if(! _redis.IsConnected)    
                    await _redis.GetDatabase().PingAsync();

                var serializedValue = JsonSerializer.Serialize(value);
                return await _db.StringSetAsync(key, serializedValue, expiration);
            }catch(Exception ex){
                _logger.Error($"Lỗi khi đặt giá trị tại bộ nhớ đệm: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<T> Get<T>(string key){
            try{
                var value = await _db.StringGetAsync(key);
                return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value);
            }catch(Exception ex){
                _logger.Error($"Lỗi khi truy xuất tại bộ nhớ đệm: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
        public async Task<bool> Remove(string key){
            try{
                
                return await _db.KeyDeleteAsync(key);
            }catch(Exception ex){
                _logger.Error($"Lỗi khi truy xuất tại bộ nhớ đệm: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> KeyExists(string key){
            return await _db.KeyExistsAsync(key);
        }

        public async Task<bool> KeyExpire(string key, TimeSpan? expiration = null){
            try{
                if(expiration == null) return false;
                return await _db.KeyExpireAsync(key, expiration);
            }catch(Exception ex){
                _logger.Error($"Lỗi khi truy xuất tại bộ nhớ đệm: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
        #endregion

        #region ====[List]====
        public async Task<bool> ListLeftPush<T>(string key, T value){
            try{
                var serializedValue = JsonSerializer.Serialize(value);
                return await _db.ListLeftPushAsync(key, serializedValue) > 0;
            }catch(Exception ex){
                _logger.Error($"Lỗi khi thêm phần tử vào danh sách ở bên trái: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> ListRightPush<T>(string key, T value){
            try{
                var serializedValue = JsonSerializer.Serialize(value);
                return await _db.ListRightPushAsync(key, serializedValue) > 0;
            }catch(Exception ex){
                _logger.Error($"Lỗi khi thêm phần tử vào danh sách ở bên phải: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> ListLeftPop<T>(string key){
            try{
                
                var value = await _db.ListLeftPopAsync(key);
                if(value.IsNullOrEmpty) return false;
                return await _db.ListRemoveAsync(key, value) > 0;

            }catch(Exception ex){
                _logger.Error($"Lỗi khi Xóa phần tử vào danh sách ở bên phải: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> ListRightPop<T>(string key){
            try{
                
                var value = await _db.ListRightPopAsync(key);
                if(value.IsNullOrEmpty) return false;
                return await _db.ListRemoveAsync(key, value) > 0;

            }catch(Exception ex){
                _logger.Error($"Lỗi khi Xóa phần tử vào danh sách ở bên phải: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> ListRemove<T>(string key, T value){
            try{
                var serializedValue = JsonSerializer.Serialize(value);
                return await _db.ListRemoveAsync(key, serializedValue) > 0;
            }catch(Exception ex){
                _logger.Error($"Lỗi khi xóa phần tử trong danh sách: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        } 

        public async Task<IEnumerable<T>> ListRange<T>(string key, int start = 0, int end = -1){
            try{
                
                var values = await _db.ListRangeAsync(key, start, end);
                if(values.IsNullOrEmpty()) return Enumerable.Empty<T>();     // Trả về danh sách rỗng thay vì null
                
                var result = new List<T>();
                foreach(var value in values){
                    if(!value.IsNullOrEmpty){   // Kiểm tra RedisValue trước khi deserialize
                        var deserializeValue = JsonSerializer.Deserialize<T>(value.ToString());

                        if(deserializeValue != null) result.Add(deserializeValue); // Kiểm tra null sau khi deserialize
                    }
                }

                return result;
            }catch(Exception ex){
                _logger.Error($"Lỗi khi lấy danh sách theo phạm vi: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }
        public async Task<int> ListLength(string key){
            try{
                return (int) await _db.ListLengthAsync(key);
            }catch(Exception ex){
                _logger.Error($"Lỗi khi lấy độ dài danh sách: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }
        #endregion

        #region ====[SortedSet]====
        public async Task<bool> SortedSetAdd<T>(string key, double score, T value){
            try{
                var serializedValue = JsonSerializer.Serialize(value);
                return await _db.SortedSetAddAsync(key, serializedValue, score);
            }catch(Exception ex){
                _logger.Error($"Lỗi khi thêm phần tử vào trong SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }

        public async Task<bool> SortedSetRemove<T>(string key, T value){
            try{
                var serializedValue = JsonSerializer.Serialize(value);
                return await _db.SortedSetRemoveAsync(key, serializedValue);
            }catch(Exception ex){
                _logger.Error($"Lỗi khi xóa phần tử vào trong SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }

        public async Task<int> SortedSetLength(string key){
            try{
                return (int) await _db.SortedSetLengthAsync(key);
            }catch(Exception ex){
                _logger.Error($"Lỗi khi lấy độ dài của SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }

        public async Task<IEnumerable<T>> SortedSetRangeByRank<T>(string key, long start = 0, long end = -1){
            try{
                
                var values = await _db.SortedSetRangeByRankAsync(key, start, end);
                if(values.IsNullOrEmpty()) return Enumerable.Empty<T>();

                var result = new List<T>();
                
                foreach(var value in values){
                    
                    if(!value.IsNullOrEmpty){
                        var deserializeValue = JsonSerializer.Deserialize<T>(value.ToString());
                        if(deserializeValue != null) result.Add(deserializeValue); 
                    } 
                }

                return result;
            }catch(Exception ex){
                _logger.Error($"Lỗi khi lấy danh sách về Rank theo phạm vi của SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }

        public async Task<IEnumerable<T>> SortedSetRangeByScore<T>(string key, double start = 0, double end = -1){
            try{
                
                var values = await _db.SortedSetRangeByScoreAsync(key, start, end);
                if(values.IsNullOrEmpty()) return Enumerable.Empty<T>();

                var result = new List<T>();
                
                foreach(var value in values){
                    
                    if(!value.IsNullOrEmpty){
                        var deserializeValue = JsonSerializer.Deserialize<T>(value.ToString());
                        if(deserializeValue != null) result.Add(deserializeValue); 
                    } 
                }

                return result;
            }catch(Exception ex){
                _logger.Error($"Lỗi khi lấy danh sách về score theo phạm vi của SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }

        public async Task<bool> SortedSetRemoveScoreByRange(string key, double start, double end){
            try{
                return await _db.SortedSetRemoveRangeByScoreAsync(key, start, end) > 0;
            }catch(Exception ex){
                _logger.Error($"Lỗi khi xóa phần tử vào trong SortedSet theo phạm vi: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }

        public async Task<double> SortedSetGetScoreByValue(string key, string value){
            try{
                var score = await _db.SortedSetScoreAsync(key, value);
                if(score.HasValue) 
                    return score.Value;
                else
                    throw new E_commerce.Core.Exceptions.InvalidOperationException("Không tìm thấy giá trị trong SortedSet.");
            }catch(Exception ex){
                _logger.Error($"Lỗi khi lấy danh sách theo phạm vi của SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> SortedSetUpdateScore<T>(string key, double score, T value){
            try{
                var serializedValue = JsonSerializer.Serialize(value);
                return await _db.SortedSetAddAsync(key, serializedValue, score);
            }catch(Exception ex){
                _logger.Error($"Lỗi khi thêm phần tử vào trong SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            } 
        }
        #endregion
    }
}