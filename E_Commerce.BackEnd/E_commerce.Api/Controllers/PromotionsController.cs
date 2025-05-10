using E_commerce.Api.Model;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class PromotionsController: BaseApiController
    {
        #region  ===[Private Member]===
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region ===[Constructor]===
        public PromotionsController(IUnitOfWork unitOfWork) 
            => _unitOfWork = unitOfWork 
                ?? throw new ArgumentNullException(nameof(unitOfWork));
        #endregion
        
        #region ===[public members]===
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<_Promotion>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.promotions.GetAllAsync(),
                rank => Success(rank, "Lấy danh sách khuyến mãi thành công")
            );
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<_Promotion>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get_PromotionByID(string id){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.promotions.GetByIdAsync(id),
                rank => Success(rank, "Lấy thông tin khuyến mãi thành công")
            );
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create_Promotion([FromBody] _Promotion form){
            
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.promotions.AddAsync(form),
                result => Success(
                    result,
                    "Đã thêm khuyến mãi thành công"
                ) 
            );
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update_Promotion([FromBody] _Promotion form){
            
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.promotions.UpdateAsync(form),
                rank => Success(rank, "Cập nhật khuyến mãi thành công")
            );
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete_Promotion(string id){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.promotions.DeleteAsync(id),
                rank => Success(rank, "Xóa khuyến mãi thành công")
            );
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch_Promotion(string id, [FromBody] JsonPatchDocument<_Promotion> patchDoc){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.promotions.PatchAsync(id, patchDoc),
                rank => Success(rank, "Cập nhật một phần thông tin khuyến mãi thành công")
            );
        }
        #endregion
    }
}