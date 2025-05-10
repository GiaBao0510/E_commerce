using E_commerce.Api.Model;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    public class ConversationController: BaseApiController
    {
        #region  ===[Private Member]===
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region ===[Constructor]===
        public ConversationController(IUnitOfWork unitOfWork) 
            => _unitOfWork = unitOfWork 
                ?? throw new ArgumentNullException(nameof(unitOfWork));
        #endregion
        
        #region ===[public members]===
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<_Conversation>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.conversations.GetAllAsync(),
                rank => Success(rank, "Lấy danh sách cuộc trò chuyện thành công")
            );
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<_Conversation>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get_ConversationByID(string id){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.conversations.GetByIdAsync(id),
                rank => Success(rank, "Lấy thông tin cuộc trò chuyện thành công")
            );
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create_Conversation([FromBody] _Conversation form){
            
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.conversations.AddAsync(form),
                result => Success(
                    result,
                    "Đã thêm cuộc trò chuyện thành công"
                ) 
            );
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update_Conversation([FromBody] _Conversation form){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.conversations.UpdateAsync(form),
                rank => Success(rank, "Cập nhật cuộc trò chuyện thành công")
            );
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete_Conversation(string id){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.conversations.DeleteAsync(id),
                rank => Success(rank, "Xóa cuộc trò chuyện thành công")
            );
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch_Conversation(string id, [FromBody] JsonPatchDocument<_Conversation> patchDoc){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.conversations.PatchAsync(id, patchDoc),
                rank => Success(rank, "Cập nhật một phần thông tin cuộc trò chuyện thành công")
            );
        }
        #endregion
    }
}