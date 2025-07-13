using System.Text.Json;
using E_commerce.Application.Application;
using E_commerce.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace E_commerce.Infrastructure.Services.impl
{
    public class RedisServices : IRedisServices
    {
        #region ====[Private Fields]====
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly ILogger _logger;
        #endregion

        /// <summary>
        /// Hàm khởi tạo 
        /// </summary>
        public RedisServices(IConnectionMultiplexer redis, ILogger logger)
        {
            _logger = logger;
            _redis = redis;
            _db = redis.GetDatabase();
        }

        #region ====[Common ingredient]====
        public async Task<bool> Set<T>(string key, T value)
        {
            try
            {
                // Nếu không kết nối được thì thực hiện kết nối lại
                if (!_redis.IsConnected)
                    await _redis.GetDatabase().PingAsync();

                string data = value is string str ? str : JsonSerializer.Serialize(value);

                string streamKey = $"Chat:message:stream"; // Tạo khóa stream tương ứng với khóa chính
                var entries = new NameValueEntry[]{
                    new NameValueEntry("text ", "Dao nay sao roi"),
                    new NameValueEntry("from_number ", "20251254"),
                    new NameValueEntry("conversation_id", 5)
                };
                RedisValue streamId = await _db.StreamAddAsync(streamKey, entries);

                return await _db.StringSetAsync(key, data);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi đặt giá trị tại bộ nhớ đệm: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {

                // Nếu không kết nối được thì thực hiện kết nối lại
                if (!_redis.IsConnected)
                    await _redis.GetDatabase().PingAsync();

                string data = value is string str? str : JsonSerializer.Serialize(value);

                return await _db.StringSetAsync(key, data, expiration);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi đặt giá trị tại bộ nhớ đệm: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<T> Get<T>(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(key);
                if (value.IsNullOrEmpty) return default;

                if (typeof(T) == typeof(string))
                    return (T)(object)value.ToString(); // Trả về giá trị dưới dạng chuỗi

                return JsonSerializer.Deserialize<T>(value);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi truy xuất tại bộ nhớ đệm: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> Remove(string key)
        {
            try
            {
                return await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi truy xuất tại bộ nhớ đệm: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> KeyExists(string key)
        {
            return await _db.KeyExistsAsync(key);
        }

        public async Task<bool> KeyExpire(string key, TimeSpan? expiration = null)
        {
            try
            {
                if (expiration == null) return false;
                return await _db.KeyExpireAsync(key, expiration);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi truy xuất tại bộ nhớ đệm: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
        #endregion

        #region ====[List]====
        public async Task<bool> ListLeftPush<T>(string key, T value)
        {
            try
            {
                return await _db.ListLeftPushAsync(key, value.ToString()) > 0;
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi thêm phần tử vào danh sách ở bên trái: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> ListRightPush<T>(string key, T value)
        {
            try
            {
                return await _db.ListRightPushAsync(key, value.ToString()) > 0;
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi thêm phần tử vào danh sách ở bên phải: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> ListLeftPop<T>(string key)
        {
            try
            {

                var value = await _db.ListLeftPopAsync(key);
                if (value.IsNullOrEmpty) return false;
                return await _db.ListRemoveAsync(key, value) > 0;

            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi Xóa phần tử vào danh sách ở bên phải: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> ListRightPop<T>(string key)
        {
            try
            {

                var value = await _db.ListRightPopAsync(key);
                if (value.IsNullOrEmpty) return false;
                return await _db.ListRemoveAsync(key, value) > 0;

            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi Xóa phần tử vào danh sách ở bên phải: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> ListRemove<T>(string key, T value)
        {
            try
            {
                return await _db.ListRemoveAsync(key, value.ToString()) > 0;
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi xóa phần tử trong danh sách: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<IEnumerable<T>> ListRange<T>(string key, int start = 0, int end = -1)
        {
            try
            {

                var values = await _db.ListRangeAsync(key, start, end);
                if (values.IsNullOrEmpty()) return Enumerable.Empty<T>();     // Trả về danh sách rỗng thay vì null

                var result = new List<T>();
                foreach (var value in values)
                {
                    if (!value.IsNullOrEmpty)
                    {   // Kiểm tra RedisValue trước khi deserialize
                        var deserializeValue = JsonSerializer.Deserialize<T>(value.ToString());

                        if (deserializeValue != null) result.Add(deserializeValue); // Kiểm tra null sau khi deserialize
                    }
                }

                return result;
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi lấy danh sách theo phạm vi: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
        public async Task<int> ListLength(string key)
        {
            try
            {
                return (int)await _db.ListLengthAsync(key);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi lấy độ dài danh sách: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
        #endregion

        #region ====[SortedSet]====
        public async Task<bool> SortedSetAdd<T>(string key, double score, T value)
        {
            try
            {
                return await _db.SortedSetAddAsync(key, value.ToString(), score);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi thêm phần tử vào trong SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> SortedSetRemove<T>(string key, T value)
        {
            try
            {
                return await _db.SortedSetRemoveAsync(key, value.ToString());
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi xóa phần tử vào trong SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<int> SortedSetLength(string key)
        {
            try
            {
                return (int)await _db.SortedSetLengthAsync(key);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi lấy độ dài của SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<IEnumerable<T>> SortedSetRangeByRank<T>(string key, long start = 0, long end = -1)
        {
            try
            {

                var values = await _db.SortedSetRangeByRankAsync(key, start, end);
                if (values.IsNullOrEmpty()) return Enumerable.Empty<T>();

                var result = new List<T>();

                foreach (var value in values)
                {

                    if (!value.IsNullOrEmpty)
                    {
                        var deserializeValue = JsonSerializer.Deserialize<T>(value.ToString());
                        if (deserializeValue != null) result.Add(deserializeValue);
                    }
                }

                return result;
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi lấy danh sách về Rank theo phạm vi của SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<IEnumerable<T>> SortedSetRangeByScore<T>(string key, double start = 0, double end = -1)
        {
            try
            {

                var values = await _db.SortedSetRangeByScoreAsync(key, start, end);
                if (values.IsNullOrEmpty()) return Enumerable.Empty<T>();

                var result = new List<T>();

                foreach (var value in values)
                {

                    if (!value.IsNullOrEmpty)
                    {
                        var deserializeValue = JsonSerializer.Deserialize<T>(value.ToString());
                        if (deserializeValue != null) result.Add(deserializeValue);
                    }
                }

                return result;
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi lấy danh sách về score theo phạm vi của SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> SortedSetRemoveScoreByRange(string key, double start, double end)
        {
            try
            {
                return await _db.SortedSetRemoveRangeByScoreAsync(key, start, end) > 0;
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi xóa phần tử vào trong SortedSet theo phạm vi: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<double> SortedSetGetScoreByValue(string key, string value)
        {
            try
            {
                var score = await _db.SortedSetScoreAsync(key, value);
                if (score.HasValue)
                    return score.Value;
                else
                    throw new Core.Exceptions.InvalidOperationException("Không tìm thấy giá trị trong SortedSet.");
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi lấy danh sách theo phạm vi của SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> SortedSetUpdateScore<T>(string key, double score, T value)
        {
            try
            {
                return await _db.SortedSetAddAsync(key, value.ToString(), score);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi thêm phần tử vào trong SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public async Task<bool> CheckValueExistsInSortedSet(string key, string value)
        {
            try
            {
                var exists = await _db.SortedSetScoreAsync(key, value);

                if (exists.HasValue)
                    return true;

                return false;
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi kiểm tra value tồn tại trong SortedSet: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
        #endregion

        //#region ====[Static]====
        //public static ConnectionMultiplexer Connection => Con
    }
}