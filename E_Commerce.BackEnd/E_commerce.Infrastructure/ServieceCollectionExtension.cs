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
           
           //Đăng ký các dịch vụ Repository
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRankRepository, RankRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            //Đăng ký UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Đăng ký AutoMapper với tất cả profiles
            services.AddAutoMapperProfiles();

            //Đăng ký Connection Pool Monitor
            services.AddHostedService<Monitoring.ConnectionPoolMonitor>();

            //Đăng ký các dịch vụ khác
            services.AddSingleton<ICodeGenerator,CodeGenerator>();
            services.AddScoped<ICheckoForDuplicateErrors, CheckoForDuplicateErrors>();
            services.AddScoped<ICustomFormat, CustomFormat>();
            services.AddScoped<IAccountServices, AccountServices>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddSingleton<IRedisServices, RedisServices>();
            services.AddScoped<ITokenListService, TokenListService>();

            //Background service
            services.AddHostedService<TokenCleanUpServices>();
        }
    }
}