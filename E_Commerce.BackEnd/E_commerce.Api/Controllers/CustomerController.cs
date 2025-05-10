using E_commerce.Api.Model;
using E_commerce.Application.Application;
using E_commerce.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers 
{
    public class CustomerController: BaseApiController
    {
         #region ===[Private Member]===
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region ===[Constructor]===
        public CustomerController(IUnitOfWork unitOfWork) 
            => _unitOfWork = unitOfWork 
                ?? throw new ArgumentNullException(nameof(unitOfWork));
        #endregion

        #region ===[public members]===
        
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<_Customer>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllcustomer(){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.customers.GetAllAsync(),
                rank => Success(rank, "Lấy danh sách khách hàng thành công")
            );
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<_Customer>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetcustomerByID(string id){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.customers.GetByIdAsync(id),
                rank => Success(rank, "Lấy thông tin khách hàng thành công")
            );
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Createcustomer([FromBody] _Customer customer){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.customers.AddAsync(customer),
                rank => Success(rank, "Thêm thông tin khách hàng thành công")
            );
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Updatecustomer([FromBody] _Customer customer){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.customers.UpdateAsync(customer),
                rank => Success(rank, "Cập nhật thông tin khách hàng thành công")
            );
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deletecustomer(string id){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.customers.DeleteAsync(id),
                rank => Success(rank, "Xóa thông tin khách hàng thành công")
            );
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PathUsser(string id, [FromBody] JsonPatchDocument<_Customer> customer){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.customers.PatchAsync(id, customer),
                rank => Success(rank, "Cập nhật thông tin khách hàng thành công")
            );
        }

        [HttpPut("/UpdateRank/{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomerRank(string id,[FromQuery] int rank_id){
            return await ExecuteWithTransaction(
                _unitOfWork,
                async() => await _unitOfWork.customers.UpdateCustomerRank(id, rank_id),
                rank => Success(rank, "Cập nhật hạng của khách hàng thành công")
            );
        }
        #endregion
    }
}