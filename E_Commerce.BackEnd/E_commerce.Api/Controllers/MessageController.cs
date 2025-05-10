using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_commerce.Api.Model;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    public class MessageController: BaseApiController 
    {
        #region  ===[Private Member]===
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region ===[Constructor]===
        public MessageController(IUnitOfWork unitOfWork) 
            => _unitOfWork = unitOfWork 
                ?? throw new ArgumentNullException(nameof(unitOfWork));
        #endregion
        
        #region ===[public members]===
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<_Message>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.messages.GetAllAsync(),
                rank => Success(rank, "Lấy danh sách tin nhắn thành công")
            );
        }

        [HttpGet("list-messages-by-conversation")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<_Message>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListOfMessagesByConversationID([FromQuery] string conversation_id){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.messages.ListOfMessagesByConversationID(conversation_id),
                rank => Success(rank, "Lấy danh sách tin nhắn thành công")
            );
        }

        [HttpGet("list-messages-by-groupchat")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<_Message>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] string group_id){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.messages.ListOfMessagesByGroupChatID(group_id),
                rank => Success(rank, "Lấy danh sách tin nhắn thành công")
            );
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<_Message>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get_MessageByID(string id){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.messages.GetByIdAsync(id),
                rank => Success(rank, "Lấy thông tin tin nhắn thành công")
            );
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create_Message([FromBody] _Message form){
            
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.messages.AddAsync(form),
                result => Success(
                    result,
                    "Đã thêm tin nhắn thành công"
                ) 
            );
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update_Message([FromBody] _Message form){
            
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.messages.UpdateAsync(form),
                rank => Success(rank, "Cập nhật tin nhắn thành công")
            );
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete_Message(string id){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.messages.DeleteAsync(id),
                rank => Success(rank, "Xóa tin nhắn thành công")
            );
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch_Message(string id, [FromBody] JsonPatchDocument<_Message> patchDoc){

            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.messages.PatchAsync(id, patchDoc),
                rank => Success(rank, "Cập nhật một phần thông tin tin nhắn thành công")
            );
        }
        #endregion
    }
}