using E_commerce.Application.Application;
using Microsoft.Extensions.DependencyInjection;

namespace E_commerce.Infrastructure.repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        public IRoleRepository _roles;
        public IUserRepository _user;
        public IRankRepository _rank;
        public ICustomerRepository _customer;
        private bool _disposed;

        public UnitOfWork(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IRoleRepository roles => _roles ??= _serviceProvider.GetRequiredService<IRoleRepository>();
        public IUserRepository users => _user ??= _serviceProvider.GetRequiredService<IUserRepository>();
        public IRankRepository ranks => _rank ??= _serviceProvider.GetRequiredService<IRankRepository>();
        public ICustomerRepository customers => _customer ??= _serviceProvider.GetRequiredService<ICustomerRepository>(); 
        public void Dispose(){
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing){
            if(!_disposed && disposing){

                //Giải phóng tài nguyên nếu cần
                //Ví dụ: Đóng kết nối Database.
                _disposed = true;
            }
        }
    }
}