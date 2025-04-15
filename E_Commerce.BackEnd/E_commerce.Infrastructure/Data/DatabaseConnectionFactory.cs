using System.Data;
using System.Threading;
using E_commerce.Application.Application;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace E_commerce.Infrastructure.Data
{
    public class DatabaseConnectionFactory
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;
        private readonly int _retryCount = 3;
        private readonly int _retryDelayMs = 500;
        private static readonly HashSet<int> RetryableErrorCode = new HashSet<int>{
            1042,   // Không thể kết nối đến mấy kỳ máy chủ MySQL nào được chỉ định
            1043,   // Bắt tay tệ
            1045,   // Tài khoản không có quyền truy cập vào máy chủ MySQL
            1153,   // Packet quá lớn
            1158,   // Lỗi đọc packet
            1159,   // Timeout đọc packet
            2002,   // Không thể kết nối đến máy chủ MySQL
            2003,   // Không thể kết nối đến máy chủ MySQL
            2004,   // Không thể kết nối đến máy chủ MySQL
            2005,   // Máy chủ MySQL không xác đinh
            2006,   // Server đã ngắt kết nối
            2013    // Mất kết nối trong quá trình truy vấn
        };

        //Hàm khởi tạo
        public DatabaseConnectionFactory(IConfiguration configuration, ILogger logger){
            _connectionString = configuration["Database:MySQL"] ??
                throw new InvalidOperationException("Connection string 'Database:MySQL' is not configured");
            _logger = logger 
                ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Tạo kết nối với database với cơ chế retry
        /// </summary>
        public IDbConnection CreateConnection(){
            
            for(int i = 0; i < _retryCount; i++){
                
                try{
                    var connection = new MySqlConnection(_connectionString);
                    if(connection.State != ConnectionState.Open)
                        connection.Open();
                    return connection;
                }
                catch(MySqlException ex)
                {
                    //Chỉ retry lỗi kết nối, không retry với lỗi cú pháp
                    bool shouldRetry = ex is MySqlException mySqlException && RetryableErrorCode.Contains(mySqlException.Number);

                    if( i == _retryCount - 1 || shouldRetry){
                        _logger.Error($"Database connection failed after {i+1} attempts: {ex.Message}", ex);
                        throw new InvalidOperationException("Database connection failed", ex);
                    }

                    _logger.Warn($"Database connection attempt {i+1} failed: {ex.Message}. Retrying in {_retryDelayMs}ms...");
                    Thread.Sleep(_retryDelayMs);
                }
            }
            
            //Nếu không thành công sau số lần retry
            throw new TimeoutException($"Failed to connection to Database after {_retryCount} attempts");
        }

        ///<summary>
        /// Tạo kết nối đến database với cơ chế retry (async)
        ///</summary>
        public async Task<IDbConnection> CreateConnectionAsync(){
             
             for(int i = 0; i < _retryCount; i++){
                
                try{
                    var connection = new MySqlConnection(_connectionString);
                    if(connection.State != ConnectionState.Open)
                        await connection.OpenAsync();
                    return connection;
                }
                catch(MySqlException ex)
                {
                    //Chỉ retry lỗi kết nối, không retry với lỗi cú pháp
                    bool shouldRetry = ex is MySqlException mySqlException && RetryableErrorCode.Contains(mySqlException.Number);

                    if( i == _retryCount - 1 || shouldRetry){
                        _logger.Error($"Database connection failed after {i+1} attempts: {ex.Message}", ex);
                        throw new InvalidOperationException("Database connection failed", ex);
                    }

                    _logger.Warn($"Database connection attempt {i+1} failed: {ex.Message}. Retrying in {_retryDelayMs}ms...");
                    Thread.Sleep(_retryDelayMs);
                }
            }
            
            //Nếu không thành công sau số lần retry
            throw new TimeoutException($"Failed to connection to Database after {_retryCount} attempts");
        }

        /// <summary>
        /// Kiểm tra kết nối đến database có hợp lệ hay không
        /// .Nếu kết nối không hợp lệ thì sẽ trả về false
        /// </summary>
        public bool ValidateConnection(IDbConnection connection){
            
            if(connection == null || connection.State != ConnectionState.Open)
                return false;
            
            try{
                using(var cmd = connection.CreateCommand()){
                    cmd.CommandText = "SELECT 1";
                    cmd.CommandTimeout = 5; // 5 giây timeout
                    return cmd.ExecuteScalar() != null;
                }
            }catch(Exception ex){
                 _logger.Error($"Connection validation failed: {ex.Message}", ex);
                return false;
            }
        }

        public void WarmupConnectionPool(){
            try{
                List<IDbConnection> connections = new List<IDbConnection>();
                int minPoolSize = 10;   //Đồng bộ với MinPoolSize

                _logger.Info($"Warming up connection pool with {minPoolSize} connections...");

                //Tạo kết nối để làm đầy pool
                for(int i = 0; i < minPoolSize; i++){
                    var connection = CreateConnection();
                    connections.Add(connection);
                }

                _logger.Info($"Successfully established {connections.Count} initial connections");

                //Trả lại kết nối cho pool
                foreach( var connection in connections){
                    connection.Dispose();
                }
            }
            catch(Exception ex){
                _logger.Error($"Error warming up connection pool: {ex.Message}", ex);
            }
        }
    }
}