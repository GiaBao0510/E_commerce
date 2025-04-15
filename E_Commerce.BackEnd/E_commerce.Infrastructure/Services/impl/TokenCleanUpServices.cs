
using E_commerce.Application.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace E_commerce.Infrastructure.Services.impl
{
    public class TokenCleanUpServices :BackgroundService
    {
        #region ===[private member]===
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); // Set the interval to 5 minutes
        #endregion

        #region ===[constructor]===
        public TokenCleanUpServices(IServiceProvider serviceProvider, ILogger logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("TokenCleanUpServices started.");
            while(!stoppingToken.IsCancellationRequested){
                try
                {
                    await CleanUpExpiredTokens();
                    _logger.Info("Đã xóa các token hết hạn trong white-list và black-list.");
                }
                catch(Exception ex){
                    _logger.Error($"Lỗi khi xóa các token hết hạn trong white-list và black-list. Message: {ex.Message}");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task CleanUpExpiredTokens(){

            //Sử dụng scope để đảm bảo dissposing các service đúng cách
            using(var scope = _serviceProvider.CreateScope()){

                var tokenListService = scope.ServiceProvider.GetRequiredService<ITokenListService>();

                //Xóa các token hết hạn trong white-list
                await tokenListService.DeleteExpiredTokens("white_list");

                //Xóa các token hết hạn trong black-list
                await tokenListService.DeleteExpiredTokens("black_list");
            }
        }

    }
}