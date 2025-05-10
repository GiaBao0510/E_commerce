using Dapper;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.Core.Exceptions;
using E_commerce.SQL.Queries;
using Microsoft.AspNetCore.JsonPatch;

namespace E_commerce.Infrastructure.repositories
{
    public class DepartmentRepository: BaseRepository<_Department>, IDepartmentRepository
    {   
        /// <summary>
        /// Hàm khởi tạo
        /// </summary
        public DepartmentRepository(
            ILogger logger,
            IUnitOfWork unitOfWork
        ): base(unitOfWork, logger)
        { }

        /// <summary>
        /// Kiểm tra tính hợp lệ của Department
        /// </summary
        public void ValidateDeparment(_Department department){
            if(department == null || string.IsNullOrWhiteSpace(department.dep_name))
                throw new ValidationException("Thông tin phòng ban không được thiếu xót");
        }

        /// <summary>
        /// Lấy tất cả thông tin
        /// </summary>
        public override async Task<IReadOnlyList<_Department>> GetAllAsync(){
            try{
                var departments = await Connection.QueryAsync<_Department>(
                    DepartmentQueries.GetAll,
                    transaction: Transaction
                );
                return departments.ToList();
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách phòng ban", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách phòng ban");
            }
        }
        
        /// <summary>
        /// Lấy tất cả thông tin theo ID
        /// </summary>
        public override async Task<_Department> GetByIdAsync(string id){
            try{
                
                if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");
                
                var department = await Connection.QueryFirstOrDefaultAsync<_Department>(
                    DepartmentQueries.FindByID,
                    new { dep_id = id },
                    transaction: Transaction
                );

                if(department == null)
                    throw new ResourceNotFoundException($"Không tìm thấy ID phòng ban: {id}");
                return department;
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách theo ID phòng ban", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách theo ID phòng ban");
            }
        }
        
        /// <summary>
        /// Thêm thông tin
        /// </summary>
        public override async Task<string> AddAsync(_Department entity){
            try{
                var department = await Connection.ExecuteAsync(
                    DepartmentQueries.Add,
                    entity,
                    transaction: Transaction
                );
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi thêm thông tin phòng ban", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi thêm thông tin phòng ban");
            }
        }
        public override async Task<string> UpdateAsync(_Department entity){
            try{

                ValidateDeparment(entity);

                var department = await Connection.ExecuteAsync(
                    DepartmentQueries.UpdateByID,
                    entity,
                    transaction: Transaction
                );

                if(department == 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID phòng ban: {entity.dep_id}");
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi cập nhật thông tin phòng ban", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi cập nhật thông tin phòng ban");
            }
        }

        public override async Task<string> DeleteAsync(string id){
            try{
                
                if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");

                var department = await Connection.ExecuteAsync(
                    DepartmentQueries.DeleteByID,
                    new { dep_id = id },
                    transaction: Transaction
                );

                if(department == 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID phòng ban: {id}");
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi xóa thông tin phòng ban", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xóa thông tin phòng ban");
            }
        }
        public override async Task<string> PatchAsync(string id, JsonPatchDocument<_Department> patchDoc){
            
            if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");

            if(patchDoc == null)
                throw new ValidationException("Thông tin cập nhật không được bỏ trống");    
            
            var department = await GetByIdAsync(id);
            if(department == null)
                throw new ResourceNotFoundException($"Không tìm thấy ID phòng ban: {id}");

            //Áp dụng các thay đổi
            patchDoc.ApplyTo(department);

            try{
                var result = await Connection.ExecuteAsync(
                    DepartmentQueries.PatchByID,
                    department,
                    transaction: Transaction
                );
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi cập thông tin phòng ban", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xóa thông tin phòng ban");
            }
        }
    }
}