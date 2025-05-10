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
            services.AddSingleton<IRedisServices, RedisServices>();
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
            services.AddScoped<IMailjetService, MailjetEmailService>();
            services.AddScoped<IMailtrapService, MailtrapSerrvices>();
            services.AddScoped<IOTPAuthenServices, OTPAuthenServices>();

            //Background service
            services.AddHostedService<TokenCleanUpServices>();          //Chạy ngầm để xóa các token hết hạn trong white-list
        }
    }
}