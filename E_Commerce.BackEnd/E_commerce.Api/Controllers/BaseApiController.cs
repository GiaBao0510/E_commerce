using E_commerce.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BaseApiController : ControllerBase
    {
        protected IActionResult Success<T> (T data, string message = null)
        {
           var res = new ApiResponse<T>{
                Success = true,
                Result = data,
                Message = message ?? "Thao tác thành công",
                Meta = new MetaData{
                    StatusCode = 200,
                    RequestId = HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow
                }
           };

           return Ok(res);
        }

        protected IActionResult Created<T>(T data, string location, string message = null){
            
            var res = new ApiResponse<T>{
                Success = true,
                Result = data,
                Message = message ?? "Tạo mới thành công",
                Meta = new MetaData{
                    StatusCode = 200,
                    RequestId = HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow
                }
           };

           return Created(location, res);
        }

        protected IActionResult NoContent(string message = null){
            return new NoContentResult();
        } 
    }

    
}