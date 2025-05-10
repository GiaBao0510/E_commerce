using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;
using E_commerce.Core.Exceptions;
using E_commerce.Api.Model;
using E_commerce.Core.Entities;
using E_commerce.Application.Application;

namespace E_commerce.Api.Controllers
{
    [Authorize(Roles = "Admin,Manager")]                    //Chỉ có người dùng Admin mới thao tác được
    public class RoleController: BaseApiController
    {
        #region  ===[Private Member]===
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region ===[Constructor]===
        public RoleController(IUnitOfWork unitOfWork) 
            => _unitOfWork = unitOfWork 
                ?? throw new ArgumentNullException(nameof(unitOfWork));
        #endregion

        #region ===[public members]===
        
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Role>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllRoles(){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.roles.GetAllAsync(),
                roles => Success(roles, "Lấy danh sách vai trò thành công")
            );
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Role>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRoleByID(string id){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.roles.GetByIdAsync(id),
                roles => Success(roles, "Lấy thông tin vai trò thành công")
            );
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRole([FromBody] Role role){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.roles.AddAsync(role),
                roles => Success(roles, "Thêm vai trò thành công")
            );
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRole([FromBody] Role role){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.roles.UpdateAsync(role),
                roles => Success(roles, "Cập nhật vai trò thành công")
            );
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRole(string id){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.roles.DeleteAsync(id),
                roles => Success(roles, "Xóa vai trò thành công")
            );
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchRole(string id, [FromBody] JsonPatchDocument<Role> patchDoc){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.roles.PatchAsync(id, patchDoc),
                roles => Success(roles, "Cập nhật một phần thông tin vai trò thành công")
            );
        }
        #endregion
    }
}