using Dapper;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.Core.Exceptions;
using E_commerce.SQL.Queries;
using Microsoft.AspNetCore.JsonPatch;

namespace E_commerce.Infrastructure.repositories
{
    public class ProductTypeRepository: BaseRepository<_ProductType>, IProductTypeRepository
    {
         /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public ProductTypeRepository(
            ILogger logger,
            IUnitOfWork unitOfWork
        ): base(unitOfWork, logger)
        { }

        /// <summary>
        /// Kiểm tra tính hợp lệ của Department
        /// </summary
        public void ValidatePosition(_ProductType position){
            if(position == null || string.IsNullOrWhiteSpace(position.protyle_name))
                throw new ValidationException("Thông tin loại sản phẩm không được thiếu xót");
        }

        /// <summary>
        /// Lấy tất cả thông tin
        /// </summary>
        public override async Task<IReadOnlyList<_ProductType>> GetAllAsync(){
            try{
                var positions = await Connection.QueryAsync<_ProductType>(
                    ProductTypeQueries.GetAll
                );
                return positions.ToList();
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách loại sản phẩm", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách loại sản phẩm");
            }
        }
        
        /// <summary>
        /// Lấy tất cả thông tin theo ID
        /// </summary>
        public override async Task<_ProductType> GetByIdAsync(string id){
            try{
                
                if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");
                
                var position = await Connection.QueryFirstOrDefaultAsync<_ProductType>(
                    ProductTypeQueries.findByID,
                    new { protyle_id = id }
                );

                if(position == null)
                    throw new ResourceNotFoundException($"Không tìm thấy ID loại sản phẩm: {id}");
                return position;
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách theo ID loại sản phẩm", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách theo ID loại sản phẩm");
            }
        }
        
        /// <summary>
        /// Thêm thông tin
        /// </summary>
        public override async Task<string> AddAsync(_ProductType entity){
            try{
                var position = await Connection.ExecuteAsync(
                    ProductTypeQueries.Add,
                    entity,
                    transaction: Transaction
                );
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi thêm thông tin loại sản phẩm", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi thêm thông tin loại sản phẩm");
            }
        }

        public override async Task<string> UpdateAsync(_ProductType entity){
            try{

                ValidatePosition(entity);

                var position = await Connection.ExecuteAsync(
                    ProductTypeQueries.UpdateByID_PUT,
                    entity,
                    transaction: Transaction
                );

                if(position == 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID loại sản phẩm: {entity.protyle_id}");
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi cập nhật thông tin loại sản phẩm", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi cập nhật thông tin loại sản phẩm");
            }
        }

        public override async Task<string> DeleteAsync(string id){
            try{
                
                if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");

                var position = await Connection.ExecuteAsync(
                    ProductTypeQueries.DeleteByID,
                    new { protyle_id = id },
                    transaction: Transaction
                );

                if(position == 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID loại sản phẩm: {id}");
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi xóa thông tin loại sản phẩm", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xóa thông tin loại sản phẩm");
            }
        }
        public override async Task<string> PatchAsync(string id, JsonPatchDocument<_ProductType> patchDoc){
            
            if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");

            if(patchDoc == null)
                throw new ValidationException("Thông tin cập nhật không được bỏ trống");    
            
            var position = await GetByIdAsync(id);
            if(position == null)
                throw new ResourceNotFoundException($"Không tìm thấy ID loại sản phẩm: {id}");

            //Áp dụng các thay đổi
            patchDoc.ApplyTo(position);

            try{
                var result = await Connection.ExecuteAsync(
                    ProductTypeQueries.UpdateByID_PATCH,
                    position,
                    transaction: Transaction
                );
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi cập thông tin loại sản phẩm", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xóa thông tin loại sản phẩm");
            }
        }
    }
}