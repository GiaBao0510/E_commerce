namespace E_commerce.Infrastructure.Services
{
    public interface IRedisServices
    {
        Task<bool> Set<T>(string key, T value);
        Task<bool> Set<T>(string key, T value, TimeSpan? expiration = null);
        Task<T> Get<T>(string key);
        Task<bool> Remove(string key);
        Task<bool> KeyExists(string key);
        Task<bool> KeyExpire(string key, TimeSpan? expiration = null);

        //list
        Task<bool> ListLeftPush<T>(string key, T value);
        Task<bool> ListRightPush<T>(string key, T value);
        Task<bool> ListLeftPop<T>(string key);
        Task<bool> ListRightPop<T>(string key);
        Task<bool> ListRemove<T>(string key, T value);
        Task<IEnumerable<T>> ListRange<T>(string key, int start = 0, int end = -1);
        Task<int> ListLength(string key);

        //SortedSet
        Task<bool> SortedSetAdd<T>(string key, double score, T value);
        Task<bool> SortedSetRemove<T>(string key, T value);
        Task<int> SortedSetLength(string key);
        Task<double> SortedSetGetScoreByValue(string key, string value);
        Task<bool> SortedSetUpdateScore<T>(string key, double score, T value);
        Task<IEnumerable<T>> SortedSetRangeByRank<T>(string key, long start = 0, long end = -1);
        Task<IEnumerable<T>> SortedSetRangeByScore<T>(string key, double start = 0, double end = -1);
        Task<bool> SortedSetRemoveScoreByRange(string key, double start, double end);
        Task<bool> CheckValueExistsInSortedSet(string key, string value);
    }
}