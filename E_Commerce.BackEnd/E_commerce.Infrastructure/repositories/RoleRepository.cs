using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using E_commerce.Core.Exceptions;
using E_commerce.Infrastructure.Constants;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.SQL.Queries;
using E_commerce.Infrastructure.Data;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace E_commerce.Infrastructure.repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        /// <summary>
        /// Khởi tạo repository với các dependencies cần thiết.
        /// </summary>
        public RoleRepository(
            IUnitOfWork unitOfWork,
            ILogger logger
        ): base(unitOfWork, logger){ }

        /// <summary>
        /// Kiểm tra tính hợp lệ của Role
        /// </summary>
        private void ValidateRole(Role role){
            if(role == null)
                throw new ValidationException("Role không được để trống");

            if(string.IsNullOrWhiteSpace(role.role_name))
                throw new ValidationException("Tên vai trò không được để trống");
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ ID của Role
        /// </summary>
        private void ValidateRoleId(string id){
            if(string.IsNullOrWhiteSpace(id))
                throw new ValidationException("ID không được bỏ trống");
        }

        /// <summary>
        /// Parse chuỗi ID thành số sbyte
        /// </summary>
        private sbyte ParseRoleId(string id){
            try{
                return sbyte.Parse(id);
            }
            catch(FormatException ex){
                throw new ValidationException($"ID không hợp lệ: {ex.Message}");
            }
            catch(OverflowException ex){
                throw new ValidationException($"ID vượt quá giới hạn cho phép: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách các Role
        /// </summary>
        public override async Task<IReadOnlyList<Role>> GetAllAsync(){
            
            try{
                
                var query = RoleQueries.AllRole;
                var result = await Connection.QueryAsync<Role>(
                    query,
                    transaction: Transaction
                );
                return result.ToList();

            }catch(DbUpdateException ex){
                _logger.Error($"Database error when retrieving all roles", ex);
                throw new DatabaseException("Lỗi khi truy vấn danh sách vai trò");
            }
            catch(Exception ex){
                _logger.Error($"Error retrieving all roles: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
        
        /// <summary>
        /// Lấy Role dựa trên ID
        /// </summary>
        public override async Task<Role> GetByIdAsync(string id){
            
            ValidateRoleId(id);

            try{
                var roleId = ParseRoleId(id);
                var result = await Connection
                    .QuerySingleOrDefaultAsync<Role>(
                        RoleQueries.RoleByID,
                        new {role_id = roleId },
                        transaction: Transaction 
                    );
                
                if(result == null)
                    throw new ResourceNotFoundException($"Không tìm thấy vai trò dựa trên ID: {id}");

                return result;
            }
            catch(MySqlException ex){

                _logger.Error($"Database error when retrieving roleId: {id}, Error Number: {ex.Number}, Message:{ex.Message}", ex);
                throw new DatabaseException("Lỗi khi truy vấn vai trò dựa trên ID");
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error retrieving roles with ID: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Thêm một Role mới vào cơ sở dữ liệu
        /// </summary>
        public override async Task<string> AddAsync(Role entity){
            
            ValidateRole(entity);
        
            try{

                var result = await Connection.ExecuteAsync(
                    RoleQueries.AddRole, entity, transaction: Transaction);

                if(result <= 0)
                    throw new DatabaseException("Lỗi. Không thể tạo vai trò");

                return "SUCCESS";

            }
            catch(MySqlException ex){

                //Kiểm tra lỗi trùng khóa 
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR){
                    _logger.Error($"Duplicate key error when adding role: {ex.Message}", ex);
                    throw new ResourceConflictException("Vai trò đã tồn tại");
                }

                throw new DatabaseException("Lỗi khi thêm vai trò vào cơ sở dữ liệu", ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error adding role: {ex.Message} \n Details: {ex.Message} ", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Cập nhật vai trò trong cơ sở dữ liệu
        /// </summary>
        public override async Task<string> UpdateAsync(Role entity){
            
            ValidateRole(entity);

            try{
                
                var result = await Connection.ExecuteAsync(
                    RoleQueries.UpdateRole, entity, transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy Role: {entity.role_id}");

                return "SUCCESS";
            }
            catch(MySqlException ex){
            
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR)
                    throw new ResourceConflictException("ID vai trò bị trùng lặp");

                _logger.Error($"Database error when updating role \n Error number:{ex.Number} \nMessage:{ex.Message}", ex);
                throw new DatabaseException("Lỗi khi cập nhật vai trò vào cơ sở dữ liệu", ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error updating role: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Xóa vai trò dựa trên ID
        /// </summary>
        public override async Task<string> DeleteAsync(string id){

            ValidateRoleId(id);

            try{
                var result = await Connection.ExecuteAsync(
                    RoleQueries.DeleteRole,
                    new { role_id = id},
                    transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy Role: {id}");

                return "SUCCESS";
            }
            catch (ResourceNotFoundException)
            {
                throw;
            }
            catch (MySqlException ex)
            {
                // Ghi đầy đủ thông tin lỗi bao gồm số lỗi
                 _logger.Error($"Database error when deleting role with ID {id} MySQL error #{ex.Number}: {ex.Message}", ex);

                // Kiểm tra lỗi foreign key (error 1451)
                if (ex.Number == MysqlExceptionsConstants.MYSQL_FOREIGN_KEY_CONSTRAINT_ERROR)
                    throw new ResourceConflictException("Không thể xóa vai trò này vì đang được sử dụng");
                
                throw new DetailsOfTheException(ex);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Error deleting role with ID {id}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public override async Task<string> PatchAsync(string id, JsonPatchDocument<Role> patchDoc){
            
            ValidateRoleId(id);

            if(patchDoc == null)
                throw new ValidationException("Dữ liệu cần cập nhật không được bỏ trống");
            
            try{
                
                //Kiểm thử RoleId có tồn tại không
                var role = await GetByIdAsync(id);
                if(role == null)
                    return "NOTFOUND";
                
                //Áp dụng các thay đổi
                patchDoc.ApplyTo(role);

                //Cập nhât cơ sở dữ liệu
                var result = await Connection.ExecuteAsync(
                    RoleQueries.UpdateRole, role, transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Role: {id}");

                return "SUCCESS";
            }
            catch (ResourceNotFoundException)
            {
                throw;
            }
            catch (MySqlException ex)
            {
                // Ghi đầy đủ thông tin lỗi bao gồm số lỗi
                _logger.Error($"MySQL error #{ex.Number}: {ex.Message}", ex);
                throw new DatabaseException("Lỗi khi cập nhật một phần thông tin vai trò");
            }
            catch(Exception ex){
                _logger.Error($"Error patching role: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
    }
}