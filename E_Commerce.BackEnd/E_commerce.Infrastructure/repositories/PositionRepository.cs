using Dapper;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.Core.Exceptions;
using E_commerce.SQL.Queries;
using Microsoft.AspNetCore.JsonPatch;

namespace E_commerce.Infrastructure.repositories
{
    public class PositionRepository: BaseRepository<_Position>, IPositionRepository
    {
        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public PositionRepository(
            ILogger logger,
            IUnitOfWork unitOfWork
        ): base(unitOfWork, logger)
        { }

        /// <summary>
        /// Kiểm tra tính hợp lệ của Department
        /// </summary
        public void ValidatePosition(_Position position){
            if(position == null || string.IsNullOrWhiteSpace(position.position_name))
                throw new ValidationException("Thông tin vị trí không được thiếu xót");
        }

        /// <summary>
        /// Lấy tất cả thông tin
        /// </summary>
        public override async Task<IReadOnlyList<_Position>> GetAllAsync(){
            try{
                var positions = await Connection.QueryAsync<_Position>(
                    PositionQueries.GetAll,
                    transaction: Transaction
                );
                return positions.ToList();
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách vị trí", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách vị trí");
            }
        }
        
        /// <summary>
        /// Lấy tất cả thông tin theo ID
        /// </summary>
        public override async Task<_Position> GetByIdAsync(string id){
            try{
                
                if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");
                
                var position = await Connection.QueryFirstOrDefaultAsync<_Position>(
                    PositionQueries.FindByID,
                    new { position_id = id },
                    transaction: Transaction
                );

                if(position == null)
                    throw new ResourceNotFoundException($"Không tìm thấy ID vị trí: {id}");
                return position;
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi lấy danh sách theo ID vị trí", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi lấy danh sách theo ID vị trí");
            }
        }
        
        /// <summary>
        /// Thêm thông tin
        /// </summary>
        public override async Task<string> AddAsync(_Position entity){
            try{
                var position = await Connection.ExecuteAsync(
                    PositionQueries.Add,
                    entity,
                    transaction: Transaction
                );
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi thêm thông tin vị trí", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi thêm thông tin vị trí");
            }
        }

        public override async Task<string> UpdateAsync(_Position entity){
            try{

                ValidatePosition(entity);

                var position = await Connection.ExecuteAsync(
                    PositionQueries.UpdateByID,
                    entity,
                    transaction: Transaction
                );

                if(position == 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID vị trí: {entity.position_id}");
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi cập nhật thông tin vị trí", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi cập nhật thông tin vị trí");
            }
        }

        public override async Task<string> DeleteAsync(string id){
            try{
                
                if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");

                var position = await Connection.ExecuteAsync(
                    PositionQueries.DeleteByID,
                    new { position_id = id },
                    transaction: Transaction
                );

                if(position == 0)
                    throw new ResourceNotFoundException($"Không tìm thấy ID vị trí: {id}");
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi xóa thông tin vị trí", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xóa thông tin vị trí");
            }
        }
        public override async Task<string> PatchAsync(string id, JsonPatchDocument<_Position> patchDoc){
            
            if(string.IsNullOrWhiteSpace(id))
                    throw new ValidationException("ID không được bỏ trống");

            if(patchDoc == null)
                throw new ValidationException("Thông tin cập nhật không được bỏ trống");    
            
            var position = await GetByIdAsync(id);
            if(position == null)
                throw new ResourceNotFoundException($"Không tìm thấy ID vị trí: {id}");

            //Áp dụng các thay đổi
            patchDoc.ApplyTo(position);

            try{
                var result = await Connection.ExecuteAsync(
                    PositionQueries.PatchByID,
                    position,
                    transaction: Transaction
                );
                return "SUCCESS";
            }
            catch(Exception ex) when (!(ex is ECommerceException) ){
                _logger.Error("Lỗi khi cập thông tin vị trí", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi xóa thông tin vị trí");
            }
        }
    }
}