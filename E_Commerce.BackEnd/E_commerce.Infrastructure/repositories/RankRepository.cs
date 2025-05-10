using Dapper;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using E_commerce.Core.Exceptions;
using E_commerce.Infrastructure.Constants;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using E_commerce.SQL.Queries;

namespace E_commerce.Infrastructure.repositories
{
    public class RankRepository:  BaseRepository<_Rank>, IRankRepository
    {

        ///<summary>
        /// Constructor
        /// </summary>
        public RankRepository(ILogger logger, IUnitOfWork unitOfWork)
            :base(unitOfWork, logger)
        { }

        /// <summary>
        /// Kiểm tra tính hợp lệ của RANK
        /// </summary>
        private void ValidateRank(_Rank rank){
            if(rank == null)
                throw new ValidationException("RANK không được để trống");

            if(string.IsNullOrWhiteSpace(rank.rank_name))
                throw new ValidationException("Tên xếp hạng không được để trống");
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ ID của RANK
        /// </summary>
        private void ValidateRankId(string id){
            if(string.IsNullOrWhiteSpace(id))
                throw new ValidationException("ID không được bỏ trống");
        }

        /// <summary>
        /// Parse chuỗi ID thành số sbyte
        /// </summary>
        private sbyte ParseRankId(string id){
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
        /// Lấy danh sách các rank
        /// </summary>
        public override async Task<IReadOnlyList<_Rank>> GetAllAsync(){
            try{
                var query = RankQueries.AllRanks;
                var ranks = await Connection.QueryAsync<_Rank>(query, transaction: Transaction);
                return ranks.ToList();
            }
            catch(Exception ex){
                _logger.Error("Lỗi khi lấy danh sách RANKS.");
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Lấy rank dựa trên ID
        /// </summary>
        public override async Task<_Rank> GetByIdAsync(string id){
            
            ValidateRankId(id);

            try
            {
                var Rank_id = ParseRankId(id);
                var rank = await Connection
                    .QueryFirstOrDefaultAsync<_Rank>(
                        RankQueries.FindByID, 
                        new { rank_id = Rank_id},
                        transaction: Transaction
                    );
                
                if(rank == null)
                    throw new ResourceNotFoundException($"Không tìm thấy RANK với ID: {id}");

                return rank;
                
            }
            catch(MySqlException ex){

                _logger.Error($"Database error when retrieving RankId: {id}, Error Number: {ex.Number}, Message:{ex.Message}", ex);
                throw new DatabaseException("Lỗi khi truy vấn RANK dựa trên ID");
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error retrieving Ranks with ID: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }


        /// <summary>
        /// Thêm một Rank mới vào cơ sở dữ liệu
        /// </summary>
        public override async Task<string> AddAsync(_Rank rank){
            
            ValidateRank(rank);

            try{
                var query = RankQueries.AddRank;
                var result = await Connection.ExecuteAsync(query, rank, transaction: Transaction);
                return result.ToString();
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
        /// Cập nhật rank trong cơ sở dữ liệu
        /// </summary>
        public override async Task<string> UpdateAsync(_Rank entity){
            
            ValidateRank(entity);

            try{
                var result = await Connection.ExecuteAsync(
                    RankQueries.UpdateRank, entity, transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy RANK: {entity.rank_id}");

                return "SUCCESS";
            }
            catch(MySqlException ex){
            
                if(ex.Number == MysqlExceptionsConstants.MYSQL_DUPLICATE_KEY_ERROR)
                    throw new ResourceConflictException("ID RANK bị trùng lặp");

                _logger.Error($"Database error when updating Rank \n Error number:{ex.Number} \nMessage:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex,"Lỗi khi cập nhật RANK vào cơ sở dữ liệu");
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error updating Rank: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Xóa RANK dựa trên ID
        /// </summary>
        public override async Task<string> DeleteAsync(string id){

            ValidateRankId(id);

            try{
                var Rank_id = ParseRankId(id);
                var result = await Connection.ExecuteAsync(
                    RankQueries.DeleteRank,
                    new { rank_id = Rank_id},
                    transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"Không tìm thấy Rank: {id}");

                return "SUCCESS";
            }
            catch (MySqlException ex)
            {
                // Ghi đầy đủ thông tin lỗi bao gồm số lỗi
                 _logger.Error($"Database error when deleting Rank with ID {id} MySQL error #{ex.Number}: {ex.Message}", ex);

                // Kiểm tra lỗi foreign key (error 1451)
                if (ex.Number == MysqlExceptionsConstants.MYSQL_FOREIGN_KEY_CONSTRAINT_ERROR)
                    throw new ResourceConflictException("Không thể xóa RANK này vì đang được sử dụng");
                
                throw new DetailsOfTheException(ex);
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Error deleting Rank with ID {id}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        public override async Task<string> PatchAsync(string id, JsonPatchDocument<_Rank> patchDoc){
            
            ValidateRankId(id);

            if(patchDoc == null)
                throw new ValidationException("Dữ liệu cần cập nhật không được bỏ trống");
            
            try{
                
                //Kiểm thử RANKId có tồn tại không
                var RANK = await GetByIdAsync(id);
                if(RANK == null)
                    throw new ResourceNotFoundException($"Không tìm thấy RANK với ID: {id}");
                
                //Áp dụng các thay đổi
                patchDoc.ApplyTo(RANK);

                //Cập nhât cơ sở dữ liệu
                var result = await Connection.ExecuteAsync(
                    RankQueries.PatchRank, RANK, transaction: Transaction
                );

                if(result <= 0)
                    throw new ResourceNotFoundException($"RANK: {id}");

                return "SUCCESS";
            }
            catch (MySqlException ex)
            {
                // Ghi đầy đủ thông tin lỗi bao gồm số lỗi
                _logger.Error($"MySQL error #{ex.Number}: {ex.Message}", ex);
                throw new DatabaseException("Lỗi khi cập nhật một phần thông tin vai trò");
            }
            catch(Exception ex){
                _logger.Error($"Error patching RANK: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
    }
}