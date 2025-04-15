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
    public class RankController: BaseApiController
    {
        #region  ===[Private Member]===
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region ===[Constructor]===
        public RankController(IUnitOfWork unitOfWork) 
            => _unitOfWork = unitOfWork 
                ?? throw new ArgumentNullException(nameof(unitOfWork));
        #endregion
        
        #region ===[public members]===
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<_Rank>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll_Ranks(){

            var _Ranks = await _unitOfWork.ranks.GetAllAsync();
            return Success(_Ranks, "Lấy danh sách xếp hạng thành công");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<_Rank>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get_RankByID(string id){

            var _Rank = await _unitOfWork.ranks.GetByIdAsync(id);
            return Success(_Rank, "Lấy thông tin xếp hạng thành công");
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create_Rank([FromBody] _Rank _Rank){
            
            var result = await _unitOfWork.ranks.AddAsync(_Rank);
            return Created(
                result,
                $"/api/v{HttpContext.GetRequestedApiVersion().ToString()}/_Rank/{_Rank.rank_id}",
                "Thêm xếp hạng thành công"
            );
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update_Rank([FromBody] _Rank _Rank){
            
            var result = await _unitOfWork.ranks.UpdateAsync(_Rank);
            return Success(result, "Cập nhật xếp hạng thành công");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete_Rank(string id){
            
            var result = await _unitOfWork.ranks.DeleteAsync(id);
            return Success(result, "Xóa xếp hạng thành công");
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch_Rank(string id, [FromBody] JsonPatchDocument<_Rank> patchDoc){
            
            var result = await _unitOfWork.ranks.PatchAsync(id, patchDoc);
            return Success(result, "Cập nhật một phần thông tin xếp hạng thành công");
        }
        #endregion
    }
}