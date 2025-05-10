using Dapper;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.Core.Exceptions;
using E_commerce.Infrastructure.Constants;
using E_commerce.Infrastructure.Utils;
using E_commerce.SQL.Queries;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace E_commerce.Infrastructure.repositories
{
    public class StaffRepository: BaseRepository<_Staff>, IStaffRepository
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Hàm khởi tạo
        /// </summary> 
        public StaffRepository(
            ILogger logger,
            IUnitOfWork unitOfWork,
            IUserRepository userRepository
        ): base(unitOfWork, logger)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Lấy danh sách nhân viên
        /// </summary>
        public override async Task<IReadOnlyList<_Staff>> GetAllAsync(){
            try
            {
                var query = StaffQueries.GetAll;
                var staffs = await Connection.QueryAsync<_Staff>(
                    query,
                    transaction: Transaction
                );
                return staffs.ToList();
            }catch(DbUpdateException ex){
                _logger.Error($"Database error when retrieving all staff", ex);
                throw new DatabaseException("Lỗi khi truy vấn danh sách nhân viên");
            }
            catch(Exception ex){
                _logger.Error($"Error retrieving all staff: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Lấy thông tin nhân viên dựa trên ID
        /// </summary>
        public override async Task<_Staff> GetByIdAsync(string id){
            
            _userRepository.ValidateUserId(id);

            try
            {
                var result = await Connection.QueryFirstOrDefaultAsync<_Staff>(
                    StaffQueries.GetByID,
                    new { user_emp = id },
                    transaction: Transaction
                );
                
                if(result == null) 
                    throw new ResourceNotFoundException($"Không tìm thấy nhân viên với ID: {id}");
                
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
        public override async Task<string> AddAsync(_Staff staff)
        {    
            try{
                
                //thêm thông tin người dùng trước
                var Uid = await _userRepository.AddAsync(staff);

                //Thêm thông tin nhân viên sau
                await Connection.ExecuteAsync(
                    StaffQueries.Add,
                    new {
                        user_emp = Uid, 
                        account_number = staff.account_number
                    },
                    transaction: Transaction
                );
                return Uid;

            }catch(MySqlException ex){
                
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR){
                    _logger.Error($"Duplicate key error when adding staff: {ex.Message}", ex);
                    throw new ValidationException($"Lỗi trùng lặp dữ liệu: {ex.Message}");
                }
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error adding role: {ex.Message} \n Details: {ex.Message} ", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin từ Khách hàng mới vào cơ sở dữ liệu
        /// </summary>
        public override async Task<string> UpdateAsync(_Staff entity){
            try{
                _userRepository.ValidateUser(entity); 
                
                var result = await Connection.ExecuteAsync(
                    StaffQueries.UpdateByID,
                    new{
                        user_emp = entity.user_emp,
                        user_name = entity.user_name,
                        date_of_birth = entity.date_of_birth,
                        address = entity.address,
                        phone_num = entity.phone_num,
                        email = entity.email,
                        account_number = entity.account_number
                    },
                    transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy nhân viên với ID: {entity.user_emp}");

                return "SUCCESS";
            }
            catch(MySqlException ex){
                _logger.Error($"Database error when adding new Rank: {ex.Number}, Message:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error adding new Rank: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Xóa nhân viên dựa trên ID
        /// </summary>
        public override async Task<string> DeleteAsync(string id){
            return await _userRepository.DeleteAsync(id);
        } 

        /// <summary>
        /// Cập nhật nhân viên dựa trên ID
        /// </summary>
        public override async Task<string> PatchAsync(string id, JsonPatchDocument<_Staff> patchDoc){
            
            if(patchDoc == null)
                throw new ValidationException("JsonPatchDocument không được bỏ trống");

            try{
                
                //Kiểm tra xem người dùng có tồn tại không
                var staff = await GetByIdAsync(id);
                if(staff == null)
                    throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {id}");
                
                //Áp dụng các thay đổi
                patchDoc.ApplyTo(staff);

                staff.date_of_birth = CustomFormat.FormatDateOfBirth(staff.date_of_birth);

                var result = await Connection.ExecuteAsync(
                    StaffQueries.PatchStaff,
                    staff,
                    transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {id}");
                
                return "SUCCESS";
            }
            catch(DbUpdateConcurrencyException ex)
            {
                _logger.Error($"Concurrency conflict when patching staff {id}: {ex.Message}", ex);
                throw new ResourceConflictException("Xảy ra xung đột khi cập nhật dữ liệu người dùng");
            }
            catch (DbUpdateException ex)
            {
                _logger.Error($"Database error when patching staff {id}: {ex.Message}", ex);
                throw new DatabaseException("Lỗi khi cập nhật thông tin người dùng");
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Error patching staff {id}: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Kiểm tra có phải là nhân viên không
        /// </summary>
        public async Task<bool> IsStaff(string uid){
            try{
                var result = await Connection.QueryFirstOrDefaultAsync<bool>(
                    StaffQueries.IsStaff,
                    new { user_emp = uid },
                    transaction: Transaction
                );

                return result;
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Error patching staff {uid}: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        } 

    }
}