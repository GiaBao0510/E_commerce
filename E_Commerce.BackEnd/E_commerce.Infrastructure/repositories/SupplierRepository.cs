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
    public class SupplierRepository: BaseRepository<_Supplier>, ISupplierRepository
    {
        /// <summary> 
        /// Hàm khởi tạo
        /// </summary>
        public SupplierRepository(
            ILogger logger,
            IUnitOfWork unitOfWork
        ): base(unitOfWork, logger)
        { }

        /// <summary>
        /// Kiểm tra tính hợp lệ của Department
        /// </summary
        public void ValidatePosition(_Supplier position){
            if(position == null || string.IsNullOrWhiteSpace(position.sup_name))
                throw new ValidationException("Thông tin nhà sản xuất không được thiếu xót");
        }

        /// <summary>
        /// Lấy tất cả thông tin
        /// </summary>
        public override async Task<IReadOnlyList<_Supplier>> GetAllAsync(){
            try{
                var positions = await Connection.QueryAsync<_Supplier>(
                    SupplierQueries.GetAll,
                    transaction: Transaction
                );
                return positions.ToList();
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách nhà sản xuất", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách nhà sản xuất");
            }
        }
        
        /// <summary>
        /// Lấy tất cả thông tin theo ID
        /// </summary>
        public override async Task<_Supplier> GetByIdAsync(string id){
            try{
                
                if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");
                
                var position = await Connection.QueryFirstOrDefaultAsync<_Supplier>(
                    SupplierQueries.GetByID,
                    new { sup_id = id },
                    transaction: Transaction
                );

                if(position == null)
                    throw new ResourceNotFoundException($"Không tìm thấy ID nhà sản xuất: {id}");
                return position;
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách theo ID nhà sản xuất", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách theo ID nhà sản xuất");
            }
        }
        
        /// <summary>
        /// Thêm thông tin
        /// </summary>
        public override async Task<string> AddAsync(_Supplier entity){
            try{
                var position = await Connection.ExecuteAsync(
                    SupplierQueries.Insert,
                    entity,
                    transaction: Transaction
                );
                return "SUCCESS";
            }
            catch(MySqlException ex){
                
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR){
                    _logger.Error($"Duplicate key error when adding user: {ex.Message}", ex);
                    throw new ResourceConflictException($"Lỗi trùng lặp dữ liệu: {ex.Message}");
                }
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi thêm thông tin nhà sản xuất", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi thêm thông tin nhà sản xuất");
            }
        }

        public override async Task<string> UpdateAsync(_Supplier entity){
            try{

                ValidatePosition(entity);

                var position = await Connection.ExecuteAsync(
                    SupplierQueries.UpdateByID_PUT,
                    entity,
                    transaction: Transaction
                );

                if(position == 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID nhà sản xuất: {entity.sup_id}");
                return "SUCCESS";
            }
            catch(MySqlException ex){
                
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR){
                    _logger.Error($"Duplicate key error when adding user: {ex.Message}", ex);
                    throw new ResourceConflictException($"Lỗi trùng lặp dữ liệu: {ex.Message}");
                }
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi cập nhật thông tin nhà sản xuất", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi cập nhật thông tin nhà sản xuất");
            }
        }

        public override async Task<string> DeleteAsync(string id){
            try{
                
                if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");

                var position = await Connection.ExecuteAsync(
                    SupplierQueries.DeleteByID,
                    new { sup_id = id },
                    transaction: Transaction
                );

                if(position == 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID nhà sản xuất: {id}");
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi xóa thông tin nhà sản xuất", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xóa thông tin nhà sản xuất");
            }
        }
        public override async Task<string> PatchAsync(string id, JsonPatchDocument<_Supplier> patchDoc){
            
            if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");

            if(patchDoc == null)
                throw new ValidationException("Thông tin cập nhật không được bỏ trống");    
            
            var position = await GetByIdAsync(id);
            if(position == null)
                throw new ResourceNotFoundException($"Không tìm thấy ID nhà sản xuất: {id}");

            //Áp dụng các thay đổi
            patchDoc.ApplyTo(position);

            try{
                var result = await Connection.ExecuteAsync(
                    SupplierQueries.UpdateByID_PATCH,
                    position,
                    transaction: Transaction
                );
                return "SUCCESS";
            }
            catch(MySqlException ex){
                
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR){
                    _logger.Error($"Duplicate key error when adding user: {ex.Message}", ex);
                    throw new ResourceConflictException($"Lỗi trùng lặp dữ liệu: {ex.Message}");
                }
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi cập thông tin nhà sản xuất", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xóa thông tin nhà sản xuất");
            }
        }
    }
}