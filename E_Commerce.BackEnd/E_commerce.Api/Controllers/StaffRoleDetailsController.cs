using E_commerce.Api.Model;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    public class StaffRoleDetailsController: BaseApiController
    {
        #region  ===[Private Member]===
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region ===[Constructor]===
        public StaffRoleDetailsController(IUnitOfWork unitOfWork) 
            => _unitOfWork = unitOfWork 
                ?? throw new ArgumentNullException(nameof(unitOfWork));
        #endregion
        
        #region ===[public members]===
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<_StaffRoleDetails>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.staffRoleDetails.GetAllAsync(),
                rank => Success(rank, "Lấy danh sách chi tiết vai trò nhân viên thành công")
            );
        }

        [HttpGet("roles-by-staff")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Role>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRoleInforByStaffID([FromQuery] string uid){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.staffRoleDetails.GetRoleInforByStaffID(uid),
                rank => Success(rank, "Lấy danh sách vai trò của nhân viên thành công")
            );
        }

        [HttpGet("staffs-by-role")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<_Staff>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStaffInforByRoleID([FromQuery] string role_id){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.staffRoleDetails.GetStaffInforByRoleID(role_id),
                rank => Success(rank, "Lấy danh sách nhân viên dựa trên vai trò thành công")
            );
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<_StaffRoleDetails>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(string id){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.staffRoleDetails.GetByIdAsync(id),
                rank => Success(rank, "Lấy thông tin chi tiết vai trò nhân viên thành công")
            );
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create_StaffRoleDetails([FromBody] _StaffRoleDetails form){
            
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.staffRoleDetails.AddAsync(form),
                result => Success(
                    result,
                    "Đã thêm chi tiết vai trò nhân viên thành công"
                ) 
            );
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update_StaffRoleDetails([FromBody] _StaffRoleDetails form){
            
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.staffRoleDetails.UpdateAsync(form),
                rank => Success(rank, "Cập nhật chi tiết vai trò  nhân viên thành công")
            );
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAll(string id){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.staffRoleDetails.DeleteAsync(id),
                rank => Success(rank, "Xóa tất cả vai trò của một nhân viên thành công")
            );
        }

        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteStaffRoleDetails([FromQuery]string uid, [FromQuery] string oldRoleId){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.staffRoleDetails.DeleteStaffRoleDetails(uid, oldRoleId),
                rank => Success(rank, "Xóa chi tiết vai trò nhân viên thành công")
            );
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch_StaffRoleDetails(string id, [FromBody] JsonPatchDocument<_StaffRoleDetails> patchDoc){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.staffRoleDetails.PatchAsync(id, patchDoc),
                rank => Success(rank, "Cập nhật một phần thông tin chi tiết vai trò  nhân viên thành công")
            );
        }
        #endregion
    }
}