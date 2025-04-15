using System.Reflection;
using E_commerce.Application.Application;
using Microsoft.Extensions.DependencyInjection;

namespace E_commerce.Infrastructure.Mappings
{
    public static class MappingExtensions
    {
        /// <summary>
        /// Đăng ký tất cả mapping profile được đánh dấu bàng IMapFrom
        /// </summary>
        public static void AddAutoMapperProfiles(this IServiceCollection services){
            
            //Lấy assembly hiện tại
            var assembly = Assembly.GetExecutingAssembly();

            //Đăng ký AutoMapper với tất cả profiles
            services.AddAutoMapper( cfg => {
                
                //Quét tất cac các class implement IMapFrom
                var types = assembly.GetExportedTypes()
                    .Where(t => t.GetInterfaces().Contains(typeof(IMapFrom)))
                    .ToList();

                //Đăng ký tất cả profiles tìm được
                foreach(var type in types){
                    
                    //Đảm bảo type là Profile
                    if(typeof(AutoMapper.Profile).IsAssignableFrom(type)){
                        var profile = Activator.CreateInstance(type) as AutoMapper.Profile;
                        if(profile != null)
                            cfg.AddProfile(profile);
                    }
                }
            }, assembly);

        }
        
    }
}