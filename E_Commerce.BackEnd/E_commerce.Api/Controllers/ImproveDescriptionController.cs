using E_commerce.Api.Model;
using E_commerce.Application.Common.Interface;
using E_commerce.Application.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers 
{
    //[Authorize]
    public class ImproveDescriptionController : BaseApiController
    {
        #region ===[private properties]===
        private readonly IImproveDescriptionContext _improveDescriptionContext;
        #endregion

        //Hàm khởi tạo
        public ImproveDescriptionController(IImproveDescriptionContext improveDescriptionContext)
        {
            _improveDescriptionContext = improveDescriptionContext ?? throw new ArgumentNullException(nameof(improveDescriptionContext));
        }

        //Hàm cải thiện mô tả sản phẩm
        [HttpPost("improve-product-description")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImproveProductDescriptionAsync([FromBody] ImproveDescriptDTO req)
        {
            string result = await _improveDescriptionContext.ImproveDescriptionAsync("ImproveProductDescription", req.input);
            return Success(result, "Cải thiện mô tả sản phẩm thành công");
        }

        //Hàm cải thiện mô tả loại sản phẩm
        [HttpPost("improve-product-type-description")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImproveProducTypeDescriptionAsync([FromBody] ImproveDescriptDTO req)
        {
            string result = await _improveDescriptionContext.ImproveDescriptionAsync("ImproveProductTypeDescription", req.input);
            return Success(result, "Cải thiện mô tả loại sản phẩm thành công");
        }
        
        //Hàm cải thiện mô tả thông tin khuyến mãi
        [HttpPost("improve-promotional-infor-description")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImprovePromotionalInformationAsync([FromBody]ImproveDescriptDTO req)
        {
            string result = await _improveDescriptionContext.ImproveDescriptionAsync("ImprovePromotionalInformation",req.input);
            return Success(result, "Cải thiện thông tin khuyến mãi thành công");
        }
    }
}