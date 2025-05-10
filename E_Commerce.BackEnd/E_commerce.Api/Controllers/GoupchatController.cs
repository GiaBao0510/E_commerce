using E_commerce.Api.Model;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    public class GoupchatController: BaseApiController
    {
               #region  ===[Private Member]===
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region ===[Constructor]===
        public GoupchatController(IUnitOfWork unitOfWork) 
            => _unitOfWork = unitOfWork 
                ?? throw new ArgumentNullException(nameof(unitOfWork));
        #endregion
        
        #region ===[public members]===
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<_GroupChat>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll_GroupChats(){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.groupChat.GetAllAsync(),
                rank => Success(rank, "Lấy danh sách nhóm trò chuyện thành công")
            );
        }

        [HttpGet("conversations-by-user")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<_Conversation>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetConversationByUserID([FromQuery] string uid){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.groupChat.GetConversationByUserID(uid),
                rank => Success(rank, "Lấy danh sách các cuộc trò chuyện của người dùng thành công")
            );
        }

        [HttpGet("users-by-conversation")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<_User>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByConversationID([FromQuery] string conversation_id){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.groupChat.GetUserByConversationID(conversation_id),
                rank => Success(rank, "Lấy danh sách các người dùng trong cuộc trò chuyện thành công")
            );
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<_GroupChat>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get_GroupChatByID(string id){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.groupChat.GetByIdAsync(id),
                rank => Success(rank, "Lấy thông tin nhóm trò chuyện thành công")
            );
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create_GroupChat([FromBody] _GroupChat _GroupChat){
            
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.groupChat.AddAsync(_GroupChat),
                result => Success(
                    result,
                    "Đã thêm hạng thành công"
                ) 
            );
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update_GroupChat([FromBody] _GroupChat _GroupChat){
            
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.groupChat.UpdateAsync(_GroupChat),
                rank => Success(rank, "Cập nhật nhóm trò chuyện thành công")
            );
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete_GroupChat(string id){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.groupChat.DeleteAsync(id),
                rank => Success(rank, "Xóa nhóm trò chuyện thành công")
            );
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch_GroupChat(string id, [FromBody] JsonPatchDocument<_GroupChat> patchDoc){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.groupChat.PatchAsync(id, patchDoc),
                rank => Success(rank, "Cập nhật một phần thông tin nhóm trò chuyện thành công")
            );
        }
        #endregion
    }
}