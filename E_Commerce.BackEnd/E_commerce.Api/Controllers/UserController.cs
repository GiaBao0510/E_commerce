using E_commerce.Api.Model;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using Microsoft.AspNetCore.Authorization;
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
            
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.users.GetAllAsync(),
                result => Success(result, "Lấy danh sách người dùng thành công")
            );
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<_User>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByID(string id){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.users.GetByIdAsync(id),
                result => Success(result, "Lấy thông tin người dùng thành công")
            );
        }

        [HttpGet("basic-user-info")]
        [ProducesResponseType(typeof(ApiResponse<_User>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBasicUserInfo([FromQuery]string id){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.users.GetBasicUserInfo(id),
                result => Success(result, "Lấy thông tin cơ bản người dùng thành công")
            );
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] _User user){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.users.AddAsync(user),
                result => Success(result, "Thêm người dùng thành công")
            );
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser([FromBody] _User user){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.users.UpdateAsync(user),
                result => Success(result, "Cập nhật người dùng thành công")
            );
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(string id){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.users.DeleteAsync(id),
                result => Success(result, "Xóa người dùng thành công")
            );
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PathUsser(string id, [FromBody] JsonPatchDocument<_User> user){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.users.PatchAsync(id, user),
                result => Success(result, "Cập nhật người dùng thành công")
            );
        }

        //Thêm ảnh người dùng
        [HttpPost]
        [Route("photo")]
        //[Authorize]
        [RequestSizeLimit(50 * 1024 * 1024)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddImageForUser(IFormFile image, [FromQuery] string uid){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => {
                    await _unitOfWork.users.AddImageForUser(uid, image); 
                    return true;
                },
                result => Success(result, "Thêm ảnh người dùng thành công")
            );
        }

        //Xóa ảnh người dùng
        [HttpDelete]
        [Route("photo")]
        //[Authorize]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteImageForUser([FromQuery] string uid){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => {
                    await _unitOfWork.users.DeleteImageForUser(uid); 
                    return true;
                },
                result => Success(result, "Xóa ảnh người dùng thành công")
            );
        }

        //Cập nhật ảnh người dùng
        [HttpPut]
        [Route("photo")]
        //[Authorize]
        [RequestSizeLimit(50 * 1024 * 1024)] // Giới hạn kích thước tệp lên đến 50MB
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateImageForUser(IFormFile image, [FromQuery] string uid){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => {
                    await _unitOfWork.users.UpdateImageForUser(uid, image); 
                    return true;
                },
                result => Success(result, "Cập nhật ảnh người dùng thành công")
            );
        }

        #endregion
    }
}