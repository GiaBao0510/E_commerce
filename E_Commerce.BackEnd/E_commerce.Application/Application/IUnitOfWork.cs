// Purpose: Interface for Unit of Work pattern.
using System.Data;

namespace E_commerce.Application.Application
{
    public interface IUnitOfWork : IDisposable
    {
        public IRoleRepository roles {get; }
        public IUserRepository users {get; }
        public IRankRepository ranks {get; }
        public ICustomerRepository customers {get; }
        public IDepartmentRepository department {get; }
        public IPositionRepository positions {get; }
        public IStaffRepository staffs {get; }
        public IConversationRepository conversations {get; } 
        public IStaffRoleDetailsRepository staffRoleDetails {get; }
        public IGroupChatRepository groupChat {get; }
        public IMessageRepository messages {get; }
        public ISupplierRepository suppliers {get;}
        public IProductTypeRepository productTypes {get; }
        public IPromotionRepository promotions {get; }

        //Quản lý Transaction
        void BeginTransaction();
        Task BeginTransactionAsync();
        void Commit();
        Task CommitAsync();
        void Rollback();
        Task RollbackAsync();

        //Chia sẻ truy cập kết nối
        IDbConnection Connection {get;}
        IDbTransaction Transaction {get;}
    }
}