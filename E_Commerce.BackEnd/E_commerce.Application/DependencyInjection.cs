using E_commerce.Application.Common.Behavious;
using E_commerce.Application.Common.Behavious.Context;
using E_commerce.Application.Common.Behavious.Strategy;
using E_commerce.Application.Common.Interface;
using Microsoft.Extensions.DependencyInjection;
 
namespace E_commerce.Application
{
    public static class DependencyInjection 
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IImproveDescription, ImproveProductDescription>();
            services.AddSingleton<IImproveDescription, ImprovePromotionalInfor>();
            services.AddSingleton<IImproveDescription, ImproveProductTypeDescription>();

            services.AddScoped<IImproveDescriptionStrategyFactory, ImproveDescriptionStrategyFactory>();
            services.AddScoped<IImproveDescriptionContext, ImproveDescriptionContext>();

            return services; 
        }
    }
}