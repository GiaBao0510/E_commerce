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
            var result = await _unitOfWork.customers.GetAllAsync();
            return Success(result, "Lấy danh sách khách hàng thành công");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<_Customer>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetcustomerByID(string id){
            var result = await _unitOfWork.customers.GetByIdAsync(id);
            return Success(result, "Lấy thông tin khách hàng thành công");
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Createcustomer([FromBody] _Customer customer){
            
            var result = await _unitOfWork.customers.AddAsync(customer);
            return Created(
                result, 
                $"/api/v{HttpContext.GetRequestedApiVersion().ToString()}/customer/{customer.user_client}",
                "Thêm vai trò thành công"
            );
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Updatecustomer([FromBody] _Customer customer){
            var result = await _unitOfWork.customers.UpdateAsync(customer);
            return Success(result, "Lấy cập nhật khách hàng thành công");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deletecustomer(string id){
            var result = await _unitOfWork.customers.DeleteAsync(id);
            return Success(result, "Xóa khách hàng thành công");
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PathUsser(string id, [FromBody] JsonPatchDocument<_Customer> customer){
            var result = await _unitOfWork.customers.PatchAsync(id, customer);
            return Success(result, "Cập nhật khách hàng thành công");
        }

        [HttpPut("/UpdateRank/{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomerRank(string id,[FromQuery] int rank_id){
            var result = await _unitOfWork.customers.UpdateCustomerRank(id, rank_id);
            return Success(result, "Cập nhật hạng của khách hàng thành công");
        }
        #endregion
    }
}