using System.Data;
using E_commerce.Application.Application;
using E_commerce.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace E_commerce.Infrastructure.repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly DatabaseConnectionFactory _databaseConnectionFactory;
        
        private IDbConnection  _connection;
        private IDbTransaction _transaction;

        //Lazy Repository
        private IRoleRepository _roles;
        private IUserRepository _user;
        private IRankRepository _rank;
        private ICustomerRepository _customer;
        private IDepartmentRepository _department;
        private IPositionRepository _position;
        private IStaffRepository _staffs;
        private IStaffRoleDetailsRepository _staffRoleDetails;
        private IConversationRepository _conversation;
        private IGroupChatRepository _groupChat;
        private IMessageRepository _message;
        private ISupplierRepository _supplier;
        private IProductTypeRepository _productType;
        private IPromotionRepository _promotion;

        private bool _disposed = false;

        public UnitOfWork(
            IServiceProvider serviceProvider,
            ILogger logger,
            DatabaseConnectionFactory databaseConnectionFactory
        )
        {
            _serviceProvider = serviceProvider;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));

            //Tạo một kết nối ngay lập tức, nhưng chưa mở nó
            _connection = _databaseConnectionFactory.CreateConnection();
        }

        public IRoleRepository roles => _roles ??= CreateRespository<IRoleRepository>();
        public IUserRepository users => _user ??= CreateRespository<IUserRepository>();
        public IRankRepository ranks => _rank ??= CreateRespository<IRankRepository>();
        public ICustomerRepository customers => _customer ??= CreateRespository<ICustomerRepository>(); 
        public IDepartmentRepository department => _department ??= CreateRespository<IDepartmentRepository>();
        public IPositionRepository positions => _position ??= CreateRespository<IPositionRepository>();
        public IStaffRepository staffs => _staffs ??= CreateRespository<IStaffRepository>();
        public IStaffRoleDetailsRepository staffRoleDetails => _staffRoleDetails ??= CreateRespository<IStaffRoleDetailsRepository>();
        public IConversationRepository conversations => _conversation ??= CreateRespository<IConversationRepository>();
        public IGroupChatRepository groupChat => _groupChat ??= CreateRespository<IGroupChatRepository>();
        public IMessageRepository messages => _message ??= CreateRespository<IMessageRepository>();
        public ISupplierRepository suppliers => _supplier??= CreateRespository<ISupplierRepository>();
        public IProductTypeRepository productTypes => _productType ??= CreateRespository<IProductTypeRepository>();
        public IPromotionRepository promotions => _promotion ??= CreateRespository<IPromotionRepository>();

        #region  ====[Transaction Management]====
        public IDbConnection Connection{
            get{
                if(_connection.State != ConnectionState.Open)
                    _connection.Open();
                return _connection;
            }
        }

        public IDbTransaction Transaction => _transaction;

        public void BeginTransaction(){
            if(_transaction != null){
                _logger.Warn("Transaction already started.");
                return;
            }
            _transaction = Connection.BeginTransaction();
            _logger.Info("Transaction Began");
        }

        public async Task BeginTransactionAsync()
        {
            if(_transaction != null){
                _logger.Warn("Transaction already started.");
                return;
            }

            //Mở kết nối, nếu kết nối đóng
            if(_connection.State != ConnectionState.Open){
                if(_connection is MySql.Data.MySqlClient.MySqlConnection mysqlconnec)
                    await mysqlconnec.OpenAsync();
                else
                    _connection.Open();
            }

            _transaction = _connection.BeginTransaction();
            _logger.Info("Transaction Began asynchronously");
        }

        public void Commit(){
            try{
                _transaction?.Commit();
                _logger.Info("Transaction Committed");
            }
            catch(Exception ex){
                _logger.Error($"Error committing transaction: {ex.Message}");
                throw;
            }
            finally{
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public async Task CommitAsync(){
            try{
                _transaction?.Commit();
                _logger.Info("Transaction Committed");
            }
            catch(Exception ex){
                _logger.Error($"Error committing transaction: {ex.Message}");
                await RollbackAsync();
                throw;
            }
            finally{
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public void Rollback(){
            try{
                _transaction?.Rollback();
                _logger.Info("Transaction Rolledback");
            }
            finally{
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackAsync(){
            try{
                _transaction?.Rollback();
                _logger.Info("Transaction Rolledback asynchronously");
            }
            finally{
                _transaction?.Dispose();
                _transaction = null;
            }
        }
        #endregion

        //Phương thức trợ giúp tạo repository với các phụ thuộc
        private T CreateRespository<T>() where T : class{
            
            //Thử lấy các tham số hàm khởi tạo tùy chình
            if(
                typeof(T) == typeof(IRoleRepository) ||
                typeof(T) == typeof(ICustomerRepository) ||
                typeof(T) == typeof(IUserRepository) || 
                typeof(T) == typeof(IRankRepository) ||
                typeof(T) == typeof(IDepartmentRepository) ||
                typeof(T) == typeof(IPositionRepository) ||
                typeof(T) == typeof(IStaffRepository) ||
                typeof(T) == typeof(IStaffRoleDetailsRepository) ||
                typeof(T) == typeof(IConversationRepository)    ||
                typeof(T) == typeof(IGroupChatRepository) ||
                typeof(T) == typeof(IMessageRepository) ||
                typeof(T) == typeof(ISupplierRepository) ||
                typeof(T) == typeof(IProductTypeRepository) ||
                typeof(T) == typeof(IPromotionRepository)
            ){
                //Truyển UnitOfWork cho các hàm khởi tạo Repository cần nó
                return _serviceProvider.GetRequiredService<T>();
            }
            return _serviceProvider.GetRequiredService<T>();
        }
        
        public void Dispose(){
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed && disposing){

                //Giải phóng các giao dịch
                _transaction?.Dispose();
                _transaction = null;

                //Giải phóng kết nối
                _connection?.Close();
                _connection?.Dispose();
                _connection = null;


                //Giải phóng tài nguyên nếu cần
                _disposed = true;
                _logger.Info("UnitOfWork disposed");

            }
        }

        //hàm hủy
        ~UnitOfWork() => Dispose(false);
    }
}