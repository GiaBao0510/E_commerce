using Dapper;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.Core.Exceptions;
using E_commerce.SQL.Queries;
using Microsoft.AspNetCore.JsonPatch;

namespace E_commerce.Infrastructure.repositories
{
    public class PromotionRepository: BaseRepository<_Promotion>, IPromotionRepository
    {
        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public PromotionRepository(
            ILogger logger,
            IUnitOfWork unitOfWork
        ): base(unitOfWork, logger)
        { }

        /// <summary>
        /// Kiểm tra tính hợp lệ của Department
        /// </summary
        public void ValidatePosition(_Promotion position){
            if(position == null || string.IsNullOrWhiteSpace(position.promo_name))
                throw new ValidationException("Thông tin khuyến mãi không được thiếu xót");
        }

        /// <summary>
        /// Lấy tất cả thông tin
        /// </summary>
        public override async Task<IReadOnlyList<_Promotion>> GetAllAsync(){
            try{
                var positions = await Connection.QueryAsync<_Promotion>(
                    PromotionQueries.GetALL
                );
                return positions.ToList();
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách khuyến mãi", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách khuyến mãi");
            }
        }
        
        /// <summary>
        /// Lấy tất cả thông tin theo ID
        /// </summary>
        public override async Task<_Promotion> GetByIdAsync(string id){
            try{
                
                if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");
                
                var position = await Connection.QueryFirstOrDefaultAsync<_Promotion>(
                    PromotionQueries.FindByID,
                    new { promo_id = id }
                );

                if(position == null)
                    throw new ResourceNotFoundException($"Không tìm thấy ID khuyến mãi: {id}");
                return position;
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách theo ID khuyến mãi", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách theo ID khuyến mãi");
            }
        }
        
        /// <summary>
        /// Thêm thông tin
        /// </summary>
        public override async Task<string> AddAsync(_Promotion entity){
            try{
                var position = await Connection.ExecuteAsync(
                    PromotionQueries.Add,
                    entity,
                    transaction: Transaction
                );
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi thêm thông tin khuyến mãi", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi thêm thông tin khuyến mãi");
            }
        }

        public override async Task<string> UpdateAsync(_Promotion entity){
            try{

                ValidatePosition(entity);

                var position = await Connection.ExecuteAsync(
                    PromotionQueries.Update_PUT,
                    entity,
                    transaction: Transaction
                );

                if(position == 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID khuyến mãi: {entity.promo_id}");
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi cập nhật thông tin khuyến mãi", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi cập nhật thông tin khuyến mãi");
            }
        }

        public override async Task<string> DeleteAsync(string id){
            try{
                
                if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");

                var position = await Connection.ExecuteAsync(
                    PromotionQueries.DeleteByID,
                    new { promo_id = id },
                    transaction: Transaction
                );

                if(position == 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID khuyến mãi: {id}");
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi xóa thông tin khuyến mãi", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xóa thông tin khuyến mãi");
            }
        }
        public override async Task<string> PatchAsync(string id, JsonPatchDocument<_Promotion> patchDoc){
            
            if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");

            if(patchDoc == null)
                throw new ValidationException("Thông tin cập nhật không được bỏ trống");    
            
            var position = await GetByIdAsync(id);
            if(position == null)
                throw new ResourceNotFoundException($"Không tìm thấy ID khuyến mãi: {id}");

            //Áp dụng các thay đổi
            patchDoc.ApplyTo(position);

            try{
                var result = await Connection.ExecuteAsync(
                    PromotionQueries.Update_PATCH,
                    position,
                    transaction: Transaction
                );
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi cập thông tin khuyến mãi", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xóa thông tin khuyến mãi");
            }
        }
    }
}