using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Isopoh.Cryptography.Argon2;
using E_commerce.Core.Exceptions;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.Infrastructure.Data;
using E_commerce.Infrastructure.Constants;
using E_commerce.SQL.Queries;
using Dapper;

namespace E_commerce.Infrastructure.repositories
{
    public class CustomerRepository: ICustomerRepository
    {
        #region ======[Private property]=====
        private readonly ILogger _logger;
        private readonly ICodeGenerator _codeGenerator;
        private readonly IUserRepository _userRepository;
        private readonly ICustomFormat _customFormat;
        private readonly ApplicationDbContext _dbContext;
        private readonly DatabaseConnectionFactory _connectionFactory;
        private readonly IRankRepository _rankRepository;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public CustomerRepository(
            ILogger logger,
            ICodeGenerator codeGenerator,
            DatabaseConnectionFactory connectionFactory,
            ApplicationDbContext dbContext,
            IUserRepository userRepository,
            ICustomFormat customFormat,
            IRankRepository rankRepository
        ){
            _logger = logger;
            _codeGenerator = codeGenerator;
            _connectionFactory = connectionFactory;
            _dbContext = dbContext;
            _userRepository = userRepository;
            _customFormat = customFormat;
            _rankRepository = rankRepository;
        }

        /// <summary>
        /// Kiểm tra Customer có tồn tại hay không
        /// </summary>
        public async Task<_Customer> IsCustomerIdExists(string id){
            var result = await (from cdt in _dbContext.CustomerRoleDetails
                                join u in _dbContext.Users on cdt.UserClient equals u.UserId
                                join r in _dbContext.Ranks on cdt.RankId equals r.RankId
                                where cdt.UserClient == id
                                select new _Customer{
                                    user_id = u.UserId,
                                    user_name = u.UserName,
                                    date_of_birth = u.DateOfBirth.ToString(),
                                    address = u.Address,
                                    phone_num = u.PhoneNum,
                                    email = u.Email,
                                    rank_id = cdt.RankId,
                                    rank_name = r.RankName,
                                    rating_point = r.RatingPoint
            }).FirstOrDefaultAsync();

            if(result == null)
                throw new ResourceNotFoundException($"Không tìm thấy khách hàng với ID: {id}");

            return result;
        }

        /// <summary>
        /// Lấy danh sách người tiêu dùng
        /// </summary>
        public async Task<IReadOnlyList<_Customer>> GetAllAsync(){
            try
            {
                using(var connection = _connectionFactory.CreateConnection()){
                    var query = CustomerQueries.AllCustomer;
                    var users = await connection.QueryAsync<_Customer>(query);
                    return users.ToList();
                }
            }catch(DbUpdateException ex){
                _logger.Error($"Database error when retrieving all user", ex);
                throw new DatabaseException("Lỗi khi truy vấn danh sách người tiêu dùng");
            }
            catch(Exception ex){
                _logger.Error($"Error retrieving all user: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Lấy thông tin khách hàng dựa trên ID
        /// </summary>
        public async Task<_Customer> GetByIdAsync(string id){
            
            _userRepository.ValidateUserId(id);

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<_Customer>(
                    CustomerQueries.CustomerByID,
                    new { user_client = id }
                );
                
                if(result == null) 
                    throw new ResourceNotFoundException($"Không tìm thấy khách hàng với ID: {id}");
                
                return result;
            }
            catch(MySqlException ex){
                _logger.Error($"Database error when retrieving UserID: {id}, Error Number: {ex.Number}, Message:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error retrieving User with ID: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

         /// <summary>
        /// Thêm một User mới vào cơ sở dữ liệu
        /// </summary>
        public async Task<string> AddAsync(_Customer user)
        {
            using var connection = _connectionFactory.CreateConnection();
                
            using var transaction =  connection.BeginTransaction();
            try{
                
                //thêm thông tin người dùng trước
                var Uid = await _userRepository.AddAsync(user);

                //Thêm thông tin khách hàng sau
                await connection.ExecuteAsync(
                    CustomerQueries.AddCustomer + CustomerQueries.AddBasicCustomerInformation,
                    new { user_client = Uid },
                    transaction
                );

                transaction.Commit();
                return Uid;

            }catch(MySqlException ex){
                
                transaction.Rollback();
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR){
                    _logger.Error($"Duplicate key error when adding user: {ex.Message}", ex);
                    throw new ValidationException($"Lỗi trùng lặp dữ liệu: {ex.Message}");
                }
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                transaction.Rollback();
                _logger.Error($"Error adding role: {ex.Message} \n Details: {ex.Message} ", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin từ Khách hàng mới vào cơ sở dữ liệu
        /// </summary>
        public async Task<string> UpdateAsync(_Customer entity){
            _userRepository.ValidateUser(entity); 
            return await _userRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// Xóa vai trò dựa trên ID
        /// </summary>
        public async Task<string> DeleteAsync(string id){
            return await _userRepository.DeleteAsync(id);
        } 

        /// <summary>
        /// Cập nhật User dựa trên ID
        /// </summary>
        public async Task<string> PatchAsync(string id, JsonPatchDocument<_Customer> patchDoc){
            
            if(patchDoc == null)
                throw new ValidationException("JsonPatchDocument không được bỏ trống");

            try{
                
                //Kiểm tra xem người dùng có tồn tại không
                var customer = await IsCustomerIdExists(id);
                if(customer == null)
                    throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {id}");
                
                //Áp dụng các thay đổi
                patchDoc.ApplyTo(customer);

                customer.date_of_birth = _customFormat.FormatDateOfBirth(customer.date_of_birth);

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteAsync(
                    CustomerQueries.PatchCustomer,
                    customer
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {id}");
                
                return "SUCCESS";
            }
            catch(DbUpdateConcurrencyException ex)
            {
                _logger.Error($"Concurrency conflict when patching user {id}: {ex.Message}", ex);
                throw new ResourceConflictException("Xảy ra xung đột khi cập nhật dữ liệu người dùng");
            }
            catch (DbUpdateException ex)
            {
                _logger.Error($"Database error when patching user {id}: {ex.Message}", ex);
                throw new DatabaseException("Lỗi khi cập nhật thông tin người dùng");
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Error patching user {id}: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Cập nhật rank của khách hàng
        /// </summary>
        public async Task<object> UpdateCustomerRank(string id, int rank_id){
            try{
                //Kiểm tra xem người dùng có tồn tại không
                var customer = await IsCustomerIdExists(id);
                if(customer == null)
                    throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {id}");

                //Kiểm tra xem rank có tồn tại không
                var rank = await _rankRepository.GetByIdAsync(rank_id.ToString());
                if(rank == null)
                    throw new ResourceNotFoundException($"Không tìm thấy rank với ID: {rank_id}");

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteAsync(
                    CustomerQueries.UpdateCustomerRank,
                    new { user_client = id, rank_id = rank_id }
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {id}");
                
                return "SUCCESS";
            }
            catch (MySqlException ex)
            {
                _logger.Error($"Database error when updating customer rank: {ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Error updating customer rank: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
        
    }
}