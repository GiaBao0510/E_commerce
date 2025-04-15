using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BCrypt.Net;
using E_commerce.Core.Exceptions;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.Infrastructure.Data;
using E_commerce.Infrastructure.Constants;
using E_commerce.SQL.Queries;
using Dapper;
using E_commerce.Application.DTOs.Common;

namespace E_commerce.Infrastructure.repositories
{
    public class UserRepository: IUserRepository
    {
        #region ======[Private property]=====
        private readonly ILogger _logger;
        private readonly ICodeGenerator _codeGenerator;
        private readonly DatabaseConnectionFactory _connectionFactory;
        private readonly ICheckoForDuplicateErrors _checkoForDuplicateErrors;
        private readonly ICustomFormat _customFormat;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public UserRepository(
            ILogger logger,
            ICheckoForDuplicateErrors checkoForDuplicateErrors,
            ICodeGenerator codeGenerator,
            DatabaseConnectionFactory connectionFactory,
            ICustomFormat customFormat
        ){
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
            _customFormat = customFormat ?? throw new ArgumentNullException(nameof(customFormat));
            _checkoForDuplicateErrors = checkoForDuplicateErrors ?? throw new ArgumentNullException(nameof(checkoForDuplicateErrors));
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của User
        /// </summary>
        public void ValidateUser(_User user){
            if(user == null)
                throw new ValidationException("User không được bỏ trống");
            
            if(string.IsNullOrWhiteSpace(user.user_name))
                throw new ValidationException("Tên người dùng không được bỏ trống");
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ ID của User
        /// </summary>
        public void ValidateUserId(string id){
            if(string.IsNullOrWhiteSpace(id))
                throw new ValidationException("ID người dùng không được bỏ trống");
        }

        /// <summary>
        /// Kiểm tra xem ID người dùng có tồn tại không
        /// </summary>
        public async Task<_User> IsUserIdExists(string id){
            
            ValidateUserId(id);

            using( var connection = _connectionFactory.CreateConnection()){
                
                var result = await connection.QueryFirstOrDefaultAsync<_User>(
                    UserQueries.UserByID,
                    new { user_id = id }
                );
                
                if(result == null)
                    throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {id}");
                
                return result;
            }
        }

        /// <summary>
        /// Lấy danh sách người dùng
        /// </summary>
        public async Task<IReadOnlyList<_User>> GetAllAsync(){
            try
            {
                using(var connection = _connectionFactory.CreateConnection()){
                    var query = UserQueries.AllUser;
                    var users = await connection.QueryAsync<_User>(query);
                    return users.ToList();
                }
            }catch(DbUpdateException ex){
                _logger.Error($"Database error when retrieving all user", ex);
                throw new DatabaseException("Lỗi khi truy vấn danh sách người dùng");
            }
            catch(Exception ex){
                _logger.Error($"Error retrieving all user: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Lấy thông tin người dùng dựa trên ID
        /// </summary>
        public async Task<_User> GetByIdAsync(string id){
            
            ValidateUserId(id);

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<_User>(
                    UserQueries.UserByID,
                    new { user_id = id }
                );
                
                if(result == null)
                    throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {id}");
                
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
        public async Task<string> AddAsync(_User user)
        {
            //Lấy UID tự động
            var UID = _codeGenerator.GenerateUID();
            user.user_id = UID;

            ValidateUser(user);

            try{
                //Kiểm tra trùng lặp
                await _checkoForDuplicateErrors.CheckForDuplicateEmails(user.email);
                await _checkoForDuplicateErrors.CheckForDuplicatePhonenumbers(user.phone_num);

                //bắm mật khẩu
                user.pass_word = BCrypt.Net.BCrypt.HashPassword(user.pass_word);

                //Thêm dữ liệu
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteAsync(UserQueries.AddUser, user);
            
                return UID;
            }
            catch(MySqlException ex){
                
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR){
                    _logger.Error($"Duplicate key error when adding user: {ex.Message}", ex);
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
        /// Cập nhật thông tin từ User mới vào cơ sở dữ liệu
        /// </summary>
        public async Task<string> UpdateAsync(_User entity){
            
            ValidateUser(entity); 

            try
            {
                // Tìm người dùng hiện tại trong database
                var existingUser = await IsUserIdExists(entity.user_id);
                if (existingUser == null)
                    throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {entity.user_id}");
                
                //Lưu lại số điện thoại, email cũ
                string oldEmail = existingUser.email;
                string oldPhone = existingUser.phone_num;

                //Chỉ cập nhật email. Nếu giá trị mới != giá trị cũ
                if(!string.IsNullOrEmpty(entity.email) && entity.email != oldEmail){
                    await _checkoForDuplicateErrors.CheckForDuplicateWhenUpdatingEmail(entity.email);
                    existingUser.email = entity.email;
                }

                //Chỉ cập nhật số điện thoại. Nếu giá trị mới != giá trị cũ
                if(!string.IsNullOrEmpty(entity.phone_num) && entity.phone_num != oldPhone){
                    await _checkoForDuplicateErrors.CheckForDuplicateWhenUpdatingPhoneNumber(entity.phone_num);
                    existingUser.phone_num = entity.phone_num;
                }
                
                // Copy other properties
                existingUser.user_name = entity.user_name;
                existingUser.date_of_birth = _customFormat.FormatDateOfBirth(entity.date_of_birth);
                existingUser.address = entity.address;
                    
                // Lưu thay đổi
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteAsync(
                    UserQueries.UpdateUser,
                    existingUser
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {entity.user_id}");
            
                return "SUCCESS";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.Error($"Concurrency conflict when updating user {entity.user_id}: {ex.Message}", ex);
                throw new ResourceConflictException("Xảy ra xung đột khi cập nhật dữ liệu người dùng");
            }
            catch (DbUpdateException ex)
            {
                _logger.Error($"Database error when updating user {entity.user_id}: {ex.Message}", ex);
                throw new DatabaseException("Lỗi khi cập nhật thông tin người dùng");
            }
            catch(MySqlException ex){
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR){
                    _logger.Error($"Duplicate key error when adding user: {ex.Message}", ex);
                    throw new ValidationException($"Lỗi trùng lặp dữ liệu: {ex.Message}");
                }
                throw new DetailsOfTheMysqlException(ex);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Error updating user {entity.user_id}: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Xóa vai trò dựa trên ID
        /// </summary>
        public async Task<string> DeleteAsync(string id){
            ValidateUserId(id);

            try
            {
                //Kiểm tra xem người dùng có tồn tại không
                var user = await IsUserIdExists(id);
                if(user == null)
                    throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {id}");

                //Xóa người dùng
                using(var connection = _connectionFactory.CreateConnection()){
                    var result = await connection.ExecuteAsync(
                        UserQueries.DeleteUser,
                        new { user_id = id }
                    );

                    if(result <= 0)
                        throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {id}");
                
                    return "SUCCESS";
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.Error($"Database error when deleting user {id}: {ex.Message}", ex);
                throw new DatabaseException("Lỗi khi xóa người dùng");
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Error deleting user {id}: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Cập nhật User dựa trên ID
        /// </summary>
        public async Task<string> PatchAsync(string id, JsonPatchDocument<_User> patchDoc){

            ValidateUserId(id);
            
            if(patchDoc == null)
                throw new ValidationException("JsonPatchDocument không được bỏ trống");

            try{
                
                //Kiểm tra xem người dùng có tồn tại không
                var user = await IsUserIdExists(id);
                if(user == null)
                    throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {id}");
                
                //Áp dụng các thay đổi
                patchDoc.ApplyTo(user);

                user.date_of_birth = _customFormat.FormatDateOfBirth(user.date_of_birth);

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteAsync(
                    UserQueries.PatchUser,
                    user
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
        /// Lấy vai trò dựa trên ID người dùng
        /// </summary>
        public async Task<IReadOnlyList<RoleNamesDTO>> ListOfRoleNames(string uid){

            ValidateUserId(uid);

            try{
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryAsync<RoleNamesDTO>(
                    UserQueries.GetListOfRoleBasedOnUser,
                    new { user_id = uid }
                );
                return result.ToList();
            }
            catch(DbUpdateException ex){
                _logger.Error($"Database error when retrieving all user", ex);
                throw new DatabaseException($"Lỗi khi lấy danh sách vai trò dựa trên ID người dùng: {uid}");
            }
            catch(Exception ex){
                _logger.Error($"Error getting list of role names user: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Lấy mật khẩu đã băm dựa trên ID người dùng
        /// </summary>
        public async Task<string> GetHashedPasswordByUserID(string uid){

            ValidateUserId(uid);

            try{
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryFirstAsync<string>(
                    UserQueries.GetHashedPasswordByUserID,
                    new { user_id = uid }
                );
                
                return result;
            }
            catch(DbUpdateException ex){
                _logger.Error($"Database error when retrieving all user", ex);
                throw new DatabaseException($"Lỗi khi lấy danh sách vai trò dựa trên ID người dùng: {uid}");
            }
            catch(Exception ex){
                _logger.Error($"Error getting list of role names user: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
    }
}