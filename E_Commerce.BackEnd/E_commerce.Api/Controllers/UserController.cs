using E_commerce.Api.Model;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    public class UserController: BaseApiController
    { 
        #region ===[Private Member]===
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region ===[Constructor]===
        public UserController(IUnitOfWork unitOfWork) 
            => _unitOfWork = unitOfWork 
                ?? throw new ArgumentNullException(nameof(unitOfWork));
        #endregion

        #region ===[public members]===
        
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<_User>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUser(){
            var result = await _unitOfWork.users.GetAllAsync();
            return Success(result, "Lấy danh sách người dùng thành công");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<_User>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByID(string id){
            var result = await _unitOfWork.users.GetByIdAsync(id);
            return Success(result, "Lấy thông tin người dùng thành công");
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] _User user){
            
            var result = await _unitOfWork.users.AddAsync(user);
            return Created(
                result, 
                $"/api/v{HttpContext.GetRequestedApiVersion().ToString()}/User/{user.user_id}",
                "Thêm vai trò thành công"
            );
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser([FromBody] _User user){
            var result = await _unitOfWork.users.UpdateAsync(user);
            return Success(result, "Lấy cập nhật người dùng thành công");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(string id){
            var result = await _unitOfWork.users.DeleteAsync(id);
            return Success(result, "Xóa người dùng thành công");
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PathUsser(string id, [FromBody] JsonPatchDocument<_User> user){
            var result = await _unitOfWork.users.PatchAsync(id, user);
            return Success(result, "Cập nhật người dùng thành công");
        }
        #endregion
    }
}