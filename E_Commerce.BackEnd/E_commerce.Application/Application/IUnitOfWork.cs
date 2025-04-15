// Purpose: Interface for Unit of Work pattern.
namespace E_commerce.Application.Application
{
    public interface IUnitOfWork : IDisposable
    {
        public IRoleRepository roles {get; }
        public IUserRepository users {get; }
        public IRankRepository ranks {get; }
        public ICustomerRepository customers {get; }
    }
}