using E_commerce.Api.Model;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    [Authorize(Roles="Admin,Manager")]
    public class SupplierController : BaseApiController
    {
        #region  ===[Private Member]===
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region ===[Constructor]===
        public SupplierController(IUnitOfWork unitOfWork) 
            => _unitOfWork = unitOfWork 
                ?? throw new ArgumentNullException(nameof(unitOfWork));
        #endregion
        
        #region ===[public members]===
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<_Supplier>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.suppliers.GetAllAsync(),
                rank => Success(rank, "Lấy danh sách nhà sản xuất thành công")
            );
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<_Supplier>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get_SupplierByID(string id){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.suppliers.GetByIdAsync(id),
                rank => Success(rank, "Lấy thông tin nhà sản xuất thành công")
            );
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create_Supplier([FromBody] _Supplier form){
            
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.suppliers.AddAsync(form),
                result => Success(
                    result,
                    "Đã thêm nhà sản xuất thành công"
                ) 
            );
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update_Supplier([FromBody] _Supplier form){
            
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.suppliers.UpdateAsync(form),
                rank => Success(rank, "Cập nhật nhà sản xuất thành công")
            );
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete_Supplier(string id){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.suppliers.DeleteAsync(id),
                rank => Success(rank, "Xóa nhà sản xuất thành công")
            );
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch_Supplier(string id, [FromBody] JsonPatchDocument<_Supplier> patchDoc){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.suppliers.PatchAsync(id, patchDoc),
                rank => Success(rank, "Cập nhật một phần thông tin nhà sản xuất thành công")
            );
        }
        #endregion
    }
}