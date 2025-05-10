using Dapper;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.Core.Exceptions;
using E_commerce.Infrastructure.Constants;
using E_commerce.SQL.Queries;
using Microsoft.AspNetCore.JsonPatch;
using MySql.Data.MySqlClient;

namespace E_commerce.Infrastructure.repositories
{
    public class StaffRoleDetailsRepository: BaseRepository<_StaffRoleDetails>, IStaffRoleDetailsRepository
    {
        #region  ====[Private Member]===
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public StaffRoleDetailsRepository(
            IUnitOfWork unitOfWork,
            ILogger logger
        ):base(unitOfWork, logger){
           
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của ID
        /// </summary>
        private void ValidInput(string input){
            if(string.IsNullOrEmpty(input))
                throw new ValidationException("ID không được bỏ trống");
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của _StaffRoleDetails
        /// </summary>
        private async Task ValidStaffRoleDetails(_StaffRoleDetails input){
            if(input == null)
                throw new ValidationException("Chi tiết vai trò nhân viên không được bỏ trống");

            if(string.IsNullOrEmpty(input.user_emp))
                throw new ValidationException("ID nhân viên không được bỏ trống");

            if(string.IsNullOrEmpty(input.role_id.ToString()))
                throw new ValidationException("ID vai trò không được bỏ trống");
            
            //Kiểm tra xem nhân viên có tồn tại không
            bool staff = await _unitOfWork.staffs.IsStaff(input.user_emp);
            if(staff == false)
                throw new ResourceNotFoundException($"Không tìm thấy nhân viên với ID: {input.user_emp}");

            //Kiểm tra xem vai trò có tồn tại không
            if(input.newRole_id.HasValue){
                var newRole = await _unitOfWork.roles.GetByIdAsync(input.newRole_id.ToString());
                if(newRole == null)
                    throw new ResourceNotFoundException($"Không tìm thấy vai trò với ID: {input.newRole_id}");
            }

            //Kiểm tra xem vai trò có tồn tại không
            var role = await _unitOfWork.roles.GetByIdAsync(input.role_id.ToString());
            if(role == null)
                throw new ResourceNotFoundException($"Khyông tìm thấy vai trò với ID: {input.role_id}");
        }

        /// <summary>
        /// Lấy danh sách các input
        /// </summary>
        public override async Task<IReadOnlyList<_StaffRoleDetails>> GetAllAsync(){
            try{
                var query = StaffRoleDetailsQueries.GetAll;
                var inputs = await Connection.QueryAsync<_StaffRoleDetails>(query, transaction: Transaction);
                return inputs.ToList();
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error("Lỗi khi lấy danh sách Chi tiết vai trò nhân viên.");
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách các vai trò dựa trên ID người dùng
        /// </summary>
        public async Task<IReadOnlyList<Role>> GetRoleInforByStaffID(string uid){
            try{

                ValidInput(uid);

                //Kiểm tra ID nhân viên có tồn tại không
                if(await _unitOfWork.staffs.IsStaff(uid) == false)
                    throw new ResourceNotFoundException($"Không tìm thấy nhân viên với ID: {uid}");

                var query = StaffRoleDetailsQueries.GetRoleByStaffID;
                var inputs = await Connection.QueryAsync<Role>(
                    query,
                    new {user_emp = uid}, 
                    transaction: Transaction
                );

                return inputs.ToList();
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error("Lỗi khi lấy danh sách Chi tiết vai trò nhân viên.");
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách các nhân viên dựa trên RoleID
        /// </summary>
        public async Task<IReadOnlyList<_Staff>> GetStaffInforByRoleID(string role_id){
            try{
                ValidInput(role_id);

                //Kiểm tra Role_id có tồn tại không
                if(await _unitOfWork.roles.GetByIdAsync(role_id) == null)
                    throw new ResourceNotFoundException($"Không tìm thấy vai trò với ID: {role_id}");

                var query = StaffRoleDetailsQueries.GetStaffByRoleID;
                var inputs = await Connection.QueryAsync<_Staff>(
                    query, 
                    new { role_id = role_id },
                    transaction: Transaction
                );
                return inputs.ToList();
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error("Lỗi khi lấy danh sách Chi tiết vai trò nhân viên.");
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Lấy input dựa trên ID
        /// </summary>
        public override async Task<_StaffRoleDetails> GetByIdAsync(string id){
            
            ValidInput(id);

            try
            {
                var input = await Connection
                    .QueryFirstOrDefaultAsync<_StaffRoleDetails>(
                        StaffRoleDetailsQueries.GetByStaffID, 
                        new { user_emp = id},
                        transaction: Transaction
                    );
                
                if(input == null)
                    throw new ResourceNotFoundException($"Không tìm thấy Chi tiết vai trò nhân viên với ID: {id}");

                return input;
                
            }
            catch(MySqlException ex){

                _logger.Error($"Database error when retrieving StaffRoleDetailsId: {id}, Error Number: {ex.Number}, Message:{ex.Message}", ex);
                throw new DatabaseException("Lỗi khi truy vấn Chi tiết vai trò nhân viên dựa trên ID");
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error retrieving StaffRoleDetailss with ID: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }


        /// <summary>
        /// Thêm một StaffRoleDetails mới vào cơ sở dữ liệu
        /// </summary>
        public override async Task<string> AddAsync(_StaffRoleDetails input){
            
            await ValidStaffRoleDetails(input);

            try{
                var query = StaffRoleDetailsQueries.AddRoleForStaff;
                var result = await Connection.ExecuteAsync(query, input, transaction: Transaction);
                return result.ToString();
            }
            catch(MySqlException ex){

                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR)
                    throw new ResourceConflictException("Nhân viên này đã cấp vai trò rồi");

                _logger.Error($"Database error when adding new StaffRoleDetails: {ex.Number}, Message:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error adding new StaffRoleDetails: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Cập nhật StaffRoleDetails trong cơ sở dữ liệu
        /// </summary>
        public override async Task<string> UpdateAsync(_StaffRoleDetails entity){
            
            await ValidStaffRoleDetails(entity);

            try{
                var result = await Connection.ExecuteAsync(
                    StaffRoleDetailsQueries.UpdateRoleForStaff, 
                    entity, 
                    transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy Chi tiết vai trò nhân viên: {entity.user_emp}");

                return "SUCCESS";
            }
            catch(MySqlException ex){
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR)
                    throw new ResourceConflictException("Nhân viên này đã cấp vai trò rồi");

                _logger.Error($"Database error when updating StaffRoleDetails \n Error number:{ex.Number} \nMessage:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex,"Lỗi khi cập nhật Chi tiết vai trò nhân viên vào cơ sở dữ liệu");
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error updating StaffRoleDetails: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Xóa tất cả vai trò của nhân viên dựa trên ID nhân viên
        /// </summary>
        public override async Task<string> DeleteAsync(string id){

            ValidInput(id);

            try{
                var result = await Connection.ExecuteAsync(
                    StaffRoleDetailsQueries.DeleteAllRoleForStaff,
                    new { user_emp = id},
                    transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy StaffRoleDetails: {id}");

                return "SUCCESS";
            }
            catch (MySqlException ex)
            {
                // Ghi đầy đủ thông tin lỗi bao gồm số lỗi
                _logger.Error($"Database error when deleting StaffRoleDetails with ID {id} MySQL error #{ex.Number}: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Error deleting StaffRoleDetails with ID {id}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Xóa chi tiết vai trò của nhân viên
        /// </summary>
        public async Task<bool> DeleteStaffRoleDetails(string uid, string oldRoleId){

            ValidInput(uid);
            ValidInput(oldRoleId);

            try{
                var result = await Connection.ExecuteAsync(
                    StaffRoleDetailsQueries.DeleteRoleForStaff,
                    new { 
                        user_emp = uid,
                        role_id = oldRoleId
                    },
                    transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID nhân viên: {uid} hoặc RoleId: {oldRoleId}");

                return true;
            }
            catch (MySqlException ex)
            {
                // Ghi đầy đủ thông tin lỗi bao gồm số lỗi
                _logger.Error($"Database error when deleting StaffRoleDetails with ID {uid} MySQL error #{ex.Number}: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Error deleting StaffRoleDetails with ID {uid}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public override async Task<string> PatchAsync(string id, JsonPatchDocument<_StaffRoleDetails> patchDoc){
            
            ValidInput(id);

            if(patchDoc == null)
                throw new ValidationException("Dữ liệu cần cập nhật không được bỏ trống");
            
            try{
                
                //Kiểm thử Chi tiết vai trò nhân viênId có tồn tại không
                var staffRoleDetails = await GetByIdAsync(id);
                if(staffRoleDetails == null)
                    throw new ResourceNotFoundException($"Không tìm thấy Chi tiết vai trò nhân viên với ID: {id}");
                
                //Áp dụng các thay đổi
                patchDoc.ApplyTo(staffRoleDetails);

                //Cập nhât cơ sở dữ liệu
                var result = await Connection.ExecuteAsync(
                    StaffRoleDetailsQueries.UpdateRoleForStaffPatch, 
                    staffRoleDetails, 
                    transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Chi tiết vai trò nhân viên: {id}");

                return "SUCCESS";
            }
            catch (MySqlException ex)
            {
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR)
                    throw new ResourceConflictException("Nhân viên này đã cấp vai trò rồi");

                // Ghi đầy đủ thông tin lỗi bao gồm số lỗi
                _logger.Error($"MySQL error #{ex.Number}: {ex.Message}", ex);
                throw new DatabaseException("Lỗi khi cập nhật một phần thông tin vai trò");
            }
            catch(Exception ex){
                _logger.Error($"Error patching Chi tiết vai trò nhân viên: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
    }
}