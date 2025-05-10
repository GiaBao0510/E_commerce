using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using E_commerce.Core.Exceptions;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.Infrastructure.Constants;
using E_commerce.SQL.Queries;
using Dapper;
using E_commerce.Application.DTOs.Common;
using E_commerce.Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using E_commerce.Infrastructure.Services;
using E_commerce.Infrastructure.Data;

namespace E_commerce.Infrastructure.repositories
{
    public class UserRepository: BaseRepository<_User>,IUserRepository
    {
        #region ======[Private property]=====
        private readonly ICheckoForDuplicateErrors _checkoForDuplicateErrors;
        private readonly ICloudinaryServices _cloudinaryServices;
        private readonly DatabaseConnectionFactory _databaseConnectionFactory;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public UserRepository(
            ILogger logger,
            IUnitOfWork unitOfWork,
            ICloudinaryServices cloudinaryServices,
            ICheckoForDuplicateErrors checkoForDuplicateErrors,
            DatabaseConnectionFactory databaseConnectionFactory
        ):base(unitOfWork, logger){
            _cloudinaryServices = cloudinaryServices ?? throw new ArgumentNullException(nameof(cloudinaryServices));
            _checkoForDuplicateErrors = checkoForDuplicateErrors ?? throw new ArgumentNullException(nameof(checkoForDuplicateErrors));
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
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

            var result = await Connection.QueryFirstOrDefaultAsync<_User>(
                UserQueries.UserByID,
                new { user_id = id },
                transaction: Transaction
            );
            
            if(result == null)
                throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {id}");
            
            return result;
            
        }

        /// <summary>
        /// Kiểm tra xem Email người dùng có tồn tại không
        /// </summary>
        public async Task<bool> IsUserEmailExists(string email)
        {
            try{
                var result = await Connection.QueryFirstOrDefaultAsync<_User>(
                    UserQueries.FindUserByEmail,
                    new { email = email },
                    transaction: Transaction
                );

                if(result == null)
                    return false;
                return true;
            }catch(DbUpdateException ex){
                _logger.Error($"Database error when checking email: {ex.Message}", ex);
                throw new DatabaseException("Lỗi khi kiểm tra email người dùng");
            }
            catch(Exception ex) when (! (ex is ECommerceException)) {
                _logger.Error($"Error checking email: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Kiểm tra xem số điện thoại của người dùng có tồn tại không
        /// </summary>
        public async Task<bool> IsUserPhoneNumerExists(string phone_num)
        {
            try{
                var result = await Connection.QueryFirstOrDefaultAsync<_User>(
                    UserQueries.FindUserByPhoneNum,
                    new { phone_num = phone_num },
                    transaction: Transaction
                );

                if(result == null)
                    return false;
                return true;
            }catch(DbUpdateException ex){
                _logger.Error($"Database error when checking email: {ex.Message}", ex);
                throw new DatabaseException("Lỗi khi kiểm tra email người dùng");
            }
            catch(Exception ex) when (! (ex is ECommerceException)) {
                _logger.Error($"Error checking email: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách người dùng
        /// </summary>
        public override async Task<IReadOnlyList<_User>> GetAllAsync(){
            try
            {
                var users = await Connection.QueryAsync<_User>(
                    UserQueries.AllUser,
                    transaction: Transaction
                );
                return users.ToList();
            }catch(DbUpdateException ex){
                _logger.Error($"Database error when retrieving all user", ex);
                throw new DatabaseException("Lỗi khi truy vấn danh sách người dùng");
            }
            catch(Exception ex) when (! (ex is ECommerceException)){
                _logger.Error($"Error retrieving all user: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Lấy thông tin người dùng dựa trên ID
        /// </summary>
        public override async Task<_User> GetByIdAsync(string id){
            
            ValidateUserId(id);

            try
            {
                var result = await Connection.QueryFirstOrDefaultAsync<_User>(
                    UserQueries.UserByID,
                    new { user_id = id },
                    transaction: Transaction
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
        /// Lấy thông tin cơ bản người dùng
        /// </summary>
        public async Task<BasicUserInfoDTO> GetBasicUserInfo(string uid){
            try{
                
                using var connection = _databaseConnectionFactory.CreateConnection();
                BasicUserInfoDTO result = await connection.QueryFirstOrDefaultAsync<BasicUserInfoDTO>(
                    UserQueries.UserByID,
                    new {user_id = uid}
                ) ?? throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {uid}");

                var roles = await ListOfRoleNames(uid) 
                    ?? Array.Empty<RoleNamesDTO>();

                result.roles = roles.Select(r => r.role_name).ToList();
                return result;
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error retrieving User with ID: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        } 

        /// <summary>
        /// Thêm một User mới vào cơ sở dữ liệu
        /// </summary>
        public override async Task<string> AddAsync(_User user)
        {
            //Lấy UID tự động
            var UID = CodeGenerator.GenerateUID();
            user.user_id = UID;

            ValidateUser(user);

            try{
                //Kiểm tra trùng lặp
                await _checkoForDuplicateErrors.CheckForDuplicateEmails(user.email);
                await _checkoForDuplicateErrors.CheckForDuplicatePhonenumbers(user.phone_num);

                //bắm mật khẩu
                user.pass_word = BCrypt.Net.BCrypt.HashPassword(user.pass_word);

                //Thêm dữ liệu
               var result = await Connection.ExecuteAsync(
                    UserQueries.AddUser, 
                    user,
                    transaction: Transaction
                );
            
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
        public override async Task<string> UpdateAsync(_User entity){
            
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
                existingUser.date_of_birth = CustomFormat.FormatDateOfBirth(entity.date_of_birth);
                existingUser.address = entity.address;
                    
                // Lưu thay đổi
                var result = await Connection.ExecuteAsync(
                    UserQueries.UpdateUser,
                    existingUser,
                    transaction: Transaction
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
        public override async Task<string> DeleteAsync(string id){
            ValidateUserId(id);

            try
            {
                //Kiểm tra xem người dùng có tồn tại không
                var user = await IsUserIdExists(id);
                if(user == null)
                    throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {id}");

                //Xóa người dùng
                var result = await Connection.ExecuteAsync(
                    UserQueries.DeleteUser,
                    new { user_id = id },
                    transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy người dùng với ID: {id}");
            
                return "SUCCESS";
                
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
        public override async Task<string> PatchAsync(string id, JsonPatchDocument<_User> patchDoc){

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

                user.date_of_birth = CustomFormat.FormatDateOfBirth(user.date_of_birth);

                var result = await Connection.ExecuteAsync(
                    UserQueries.PatchUser,
                    user,
                    transaction: Transaction
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
                using var connection = _databaseConnectionFactory.CreateConnection();
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
            catch(Exception ex) when (! (ex is ECommerceException)){
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
                var result = await Connection.QueryFirstAsync<string>(
                    UserQueries.GetHashedPasswordByUserID,
                    new { user_id = uid },
                    transaction: Transaction
                );
                
                return result;
            }
            catch(DbUpdateException ex){
                _logger.Error($"Database error when retrieving all user", ex);
                throw new DatabaseException($"Lỗi khi lấy danh sách vai trò dựa trên ID người dùng: {uid}");
            }
            catch(Exception ex) when (! (ex is ECommerceException)){
                _logger.Error($"Error getting list of role names user: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Thêm ảnh ngươi dùng dựa trên ID người dùng
        /// </summary>
        public async Task AddImageForUser(string uid, IFormFile file){
            try{
                //Kiểm tra ID người dùng
                await IsUserIdExists(uid);

                //Thêm ảnh vào cloudinary
                var imageUrl = await _cloudinaryServices.AddImageAssync(file);

                //Thêm public_id và path_img vào trong db
                var result = await Connection.ExecuteScalarAsync<int>(
                    ImageQueries.AddImage,
                    new{
                        public_id = imageUrl.PublicId,
                        path_img = imageUrl.Url?.ToString()
                    },
                    transaction: Transaction
                );

                _logger.Info($"Thông tin ảnh");

                //Lấy image_ID dựa trên UID rồi thêm vào bảng UserPhotoDetails
                await Connection.ExecuteAsync(
                    UserPhotoDetailsQueries.AddUserPhotoDetails,
                    new{
                        user_id = uid,
                        img_id = result
                    },
                    transaction: Transaction
                );

            }
            catch(DbUpdateException ex){
                _logger.Error($"Database error when add image for use", ex);
                throw new DatabaseException($"Lỗi khi thêm ảnh dựa trên ID người dùng: {uid}");
            }
            
            catch(Exception ex) when (! (ex is ECommerceException)){
                _logger.Error($"Error when add image for user: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Xóa ảnh người dùng dựa trên ID người dùng
        /// </summary>
        public async Task DeleteImageForUser(string uid){
            try{
                //Kiểm tra ID người dùng có tồn tại không
                await IsUserIdExists(uid);

                //Lấy thông tin image dựa trên UID
                _Image imageId = await Connection.QueryFirstOrDefaultAsync<_Image>(
                    UserPhotoDetailsQueries.GetImageIdByUserId,
                    new { user_id = uid },
                    transaction: Transaction
                );

                //Xóa ảnh trong DB
                await Connection.ExecuteAsync(
                    ImageQueries.DeleteImageByID,
                    new { img_id = imageId.img_id },
                    transaction: Transaction
                );


                //Xóa ảnh trong cloudinary
                var result = await _cloudinaryServices.DeleteImageAssync(imageId.public_id);
                _logger.Info($@"Xóa ảnh trong cloudinary: 
                    status {result.StatusCode},  
                    public_id: {imageId.public_id}, 
                    img_id: {imageId.img_id}, 
                    Uid: {uid}
                ");
            }
            catch(DbUpdateException ex){
                _logger.Error($"Database error when delete user's image", ex);
                throw new DatabaseException($"Lỗi khi xóa ảnh dựa trên ID người dùng: {uid}");
            }
            
            catch(Exception ex) when (! (ex is ECommerceException)){
                _logger.Error($"Error when delete user's image: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Câp nhật ảnh người dùng dựa trên ID người dùng
        /// </summary>
        public async Task UpdateImageForUser(string uid, IFormFile file){
            try{

                //Xóa ảnh cũ của người dùng trước
                await DeleteImageForUser(uid);

                //Thêm ảnh mới vào cloudinary
                await AddImageForUser(uid, file);
            }
            catch(DbUpdateException ex){
                _logger.Error($"Database error when update user's image", ex);
                throw new DatabaseException($"Lỗi khi cập nhật ảnh dựa trên ID người dùng: {uid}");
            }
            
            catch(Exception ex) when (! (ex is ECommerceException)){
                _logger.Error($"Error when update user's image: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
    }
}