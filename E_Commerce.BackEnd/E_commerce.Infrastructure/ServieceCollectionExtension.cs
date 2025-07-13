using E_commerce.Application.Application;
using E_commerce.Infrastructure.repositories;
using E_commerce.Logging;
using E_commerce.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using E_commerce.Infrastructure.Mappings;
using E_commerce.Infrastructure.Utils;
using E_commerce.Infrastructure.Services; 
using E_commerce.Infrastructure.Services.impl;
using Mailjet.Client;
using StackExchange.Redis;
using E_commerce.Infrastructure.Services.impl.ConcreteStrategy;
using E_commerce.Infrastructure.Services.impl.Factory;

namespace E_commerce.Infrastructure
{
    public static class ServieceCollectionExtension
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration){
            
            //Đăng ký DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    configuration["Database:MySQL"],
                    ServerVersion.AutoDetect(configuration["Database:MySQL"]),
                    mySqlOptions => mySqlOptions.EnableRetryOnFailure()
                )
            );

            //Đăng ký DatabaseConnectionFactory cho Dapper
            services.AddSingleton<DatabaseConnectionFactory>();
            
            //Đăng ký Logger
            services.AddSingleton<ILogger, Logger>();

            //Đăng ký UnitOfWork trước các repository
            services.AddScoped<IUnitOfWork, UnitOfWork>();
           
           //Đăng ký các dịch vụ Repository
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRankRepository, RankRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<IStaffRoleDetailsRepository, StaffRoleDetailsRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IGroupChatRepository, GroupChatRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();

            //Đăng ký AutoMapper với tất cả profiles
            services.AddAutoMapperProfiles();

            //Đăng ký Connection Pool Monitor
            services.AddHostedService<Monitoring.ConnectionPoolMonitor>();

            //Đăng ký các dịch vụ khác
            services.AddScoped<ICheckoForDuplicateErrors, CheckoForDuplicateErrors>();
            services.AddScoped<IAccountServices, AccountServices>();
            services.AddScoped<ITokenService, TokenService>();

            /// <summary>
            /// IConnectionMultiplexer:
            /// - Đây là giao diện của StackExchange.Redis đại diện cho kết nối đến Redis
            /// </summary> 
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var RedisHost = configuration["Database:Redis:Host"];
                var redisPassword = configuration["Database:Redis:Password"];

                if(string.IsNullOrEmpty(RedisHost))
                {
                    throw new ArgumentNullException(nameof(RedisHost), "Redis host cannot be null or empty.");
                }

                //Thiết lập cấu hình
                var option = new ConfigurationOptions
                {
                    AbortOnConnectFail = false,                  //Không ngừng cố gắng kết nối lại khi không thành công
                    ConnectTimeout = 10000,                       //Thời gian chờ kết nối được tính bằng giây (nếu không được thì ném lỗi)
                    SyncTimeout = 10000,                          //Thời gian tối đa (tính bằng miliseconds) cho các lệnh đồng bộ (blocking) khi gửi đến redis chờ phản hồi
                    AsyncTimeout = 10000,                        // Thời gian tối đa (tính bằng miliseconds) cho các lệnh không đồng bộ (non-blocking) khi gửi đến redis chờ phản hồi
                    ConnectRetry = 5,                            //Số lần thử lại kết nối nếu  lần đầu thất bại
                    KeepAlive = 60,                              //Redis sẽ gửi ping signal sau mỗi 60 giây để luôn gữi kết nối luôn mở
                    ReconnectRetryPolicy = new ExponentialRetry(1000, 10000),//Tự động kết nối lại sau mỗi 1 giây nếu không thành công
                    AllowAdmin = true,                           //Cho phép Client thực hiện gửi các lệnh quản trị Redis
                    Ssl = false                                  //Dùng SSL hay không (true/false) để mã hóa kết nối với redis. (Nếu Redis nằm trên cục bộ thì nên dặt false)
                };

                //Thêm end point
                option.EndPoints.Add(RedisHost); // Cổng mặc định của Redis là 6379

                //Thêm password
                option.Password = redisPassword;

                return ConnectionMultiplexer.Connect(option);   //Trả về một kết nối đến Redis đã được cấu hình
            });
            services.AddScoped<ITokenListManagementService, TokenListManagementService>();
            services.AddSingleton<ICloudinaryServices, CloudinaryServices>();
            services.AddScoped<IGoogleServiceAuthentication, GoogleServiceAuthentication>();
            services.AddHttpClient<IGoogleServiceAuthentication, GoogleServiceAuthentication>();
            services.AddHttpClient<IMailjetClient, MailjetClient>(client => 
            {
                //Thiết lập BaseAddress, MediaType, UserAgent
                client.SetDefaultSettings();

                //Thiết lập Authentication
                client.UseBasicAuthentication(
                    configuration["Authentication:Mailjet:APIKEY_PUBLIC"], 
                    configuration["Authentication:Mailjet:APIKEY_PRIVATE"]
                );
            });
            services.AddScoped<IRedisServices, RedisServices>();
            services.AddScoped<IMailjetService, MailjetEmailService>();

            services.AddScoped<MailtrapSerrvices>();
            services.AddScoped<SmsServices>();

            services.AddScoped<ISendVerificationMessage>(provider => provider.GetService<MailtrapSerrvices>());
            services.AddScoped<ISendVerificationMessage>(provider => provider.GetService<SmsServices>());

            services.AddScoped<ISendVerificationMessageStrategyFactory, SendVerificationMessageStrategyFactory>();
            services.AddScoped<IOTPAuthenServices, OTPAuthenServices>();
            services.AddScoped<IFileDataProccessingServices, FileDataProccessingServices>();

            //Background service
            services.AddHostedService<TokenCleanUpServices>();          //Chạy ngầm để xóa các token hết hạn trong white-list
        }
    }
}