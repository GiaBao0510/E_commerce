using E_commerce.Application.Application;
using Microsoft.Extensions.Hosting;
using Dapper; 
using E_commerce.Infrastructure.Data;

namespace E_commerce.Infrastructure.Monitoring
{
    public class ConnectionPoolMonitor : BackgroundService 
    {
        private readonly ILogger _logger;
        private readonly DatabaseConnectionFactory _conectionFactory;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public ConnectionPoolMonitor(ILogger logger, DatabaseConnectionFactory conectionFactory){
            _logger = logger;
            _conectionFactory = conectionFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken){
            
            while(!stoppingToken.IsCancellationRequested){
                try
                { 
                    using(var connection = _conectionFactory.CreateConnection()){
                        
                        // //Lấy thông tin trạng thái kết nối
                        var status = await connection.QueryAsync<dynamic>("SHOW STATUS LIKE 'Threads_%';");

                        int Threads_connected = 0, Threads_created = 0, Threads_running = 0, Threads_cached = 0;
                        foreach(var row in status)
                        {
                            string variableName = row.Variable_name;
                            string valueStr = row.Value.ToString();
                            
                            switch(variableName){
                                case "Threads_connected":
                                    if(int.TryParse(valueStr, out int connectedValue))
                                        Threads_connected = connectedValue; 
                                    break;
                                case "Threads_created":
                                    if(int.TryParse(valueStr, out int createdValue))
                                        Threads_created = createdValue; 
                                    break;
                                case "Threads_running":
                                    if(int.TryParse(valueStr, out int runningValue))
                                        Threads_running = runningValue; 
                                    break;
                                case "Threads_cached":
                                    if(int.TryParse(valueStr, out int cachedValue))
                                        Threads_cached = cachedValue;
                                    break;
                            }
                        }

                        //lấy giới hạn tối đa của kết nối
                        var maxConnections = await connection.QuerySingleAsync<dynamic>("SHOW VARIABLES LIKE 'max_connections';");
                        int maxConnectionsValue =  0;
                        if(int.TryParse(maxConnections.Value.ToString(), out int maxvalue))
                            maxConnectionsValue = maxvalue;

                        // Log thông tin
                        _logger.Info($"MySQL Connection Pool Stats - " +
                            $"Connected: {Threads_connected}, " +
                            $"Running: {Threads_running}, " +
                            $"Cached: {Threads_cached}, " +
                            $"Created: {Threads_created}, " +
                            $"Max Connections: {maxConnectionsValue}");

                        //Cảnh báo nếu số kết nối đang gần giới hạn
                        if(Threads_connected > (maxConnectionsValue * 0.8))
                            _logger.Warn($"MySQL Connection Pool approaching limit - {Threads_connected}/{maxConnectionsValue} connections");
                    }
                    
                }catch(Exception ex){
                    _logger.Error("Error monitoring connection pool", ex);
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}