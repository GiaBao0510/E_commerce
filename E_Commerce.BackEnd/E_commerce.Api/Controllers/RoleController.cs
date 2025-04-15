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
    //[Authorize(Roles = "Admin")]                    //Chỉ có người dùng Admin mới thao tác được
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

            var roles = await _unitOfWork.roles.GetAllAsync();
            return Success(roles, "Lấy danh sách vai trò thành công");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Role>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRoleByID(string id){

            var role = await _unitOfWork.roles.GetByIdAsync(id);
            return Success(role, "Lấy thông tin vai trò thành công");
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRole([FromBody] Role role){
            
            var result = await _unitOfWork.roles.AddAsync(role);
            return Created(
                result,
                $"/api/v{HttpContext.GetRequestedApiVersion().ToString()}/role/{role.role_id}",
                "Thêm vai trò thành công"
            );
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRole([FromBody] Role role){
            
            var result = await _unitOfWork.roles.UpdateAsync(role);
            return Success(result, "Cập nhật vai trò thành công");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRole(string id){
            
            var result = await _unitOfWork.roles.DeleteAsync(id);
            return Success(result, "Xóa vai trò thành công");
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchRole(string id, [FromBody] JsonPatchDocument<Role> patchDoc){
            
            var result = await _unitOfWork.roles.PatchAsync(id, patchDoc);
            return Success(result, "Cập nhật một phần thông tin vai trò thành công");
        }
        #endregion
    }
}