using E_commerce.Api.Model;
using E_commerce.Application.Application;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Hàm này gửi trạng thái thành công về cho client
        /// </summary>
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


        /// <summary>
        /// Hàm này thông báo cho người dùng biết đang bị lỗi phía server
        /// </summary>
        protected IActionResult InternalError <T> (T data, string message = null)
        {
           var res = new ApiResponse<T>{
                Success = true,
                Result = data,
                Message = message ?? "Lỗi nội bộ",
                Meta = new MetaData{
                    StatusCode = 500,
                    RequestId = HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow
                }
           };

           return Problem(res.Message, res.Meta.RequestId, res.Meta.StatusCode);
        }

        /// <summary>
        /// Hàm này cho người dùng biết là đã tạo ra một trường mới thành công
        /// </summary>
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

        /// <summary>
        /// Hàm này dùng để thực hiện với giao dịch chung
        /// </summary>
        protected async Task<IActionResult> ExecuteWithTransaction<T>(
            IUnitOfWork unitOfWork,                 //UnitOfWork
            Func<Task<T>> acction,                  //Hàm bất đồng bộ thực hiện một tác vụ nào đó
            Func<T, IActionResult> successResult    //Hàm này nhận tham số T và trả về IActionResult
        ){
            try{
                //Bắt đầu giao dịch
                await unitOfWork.BeginTransactionAsync();
                
                var result = await acction();     //Thực hiện giao dịch

                //Commit thành công
                await unitOfWork.CommitAsync();

                return successResult(result);
            }
            catch
            {
                //Cuộn lại giao dịch
                await unitOfWork.RollbackAsync();
                throw;     // re-throw để middleware xử lý lỗi
            }
        }

        /// <summary>
        /// Hàm này không gửi nội dung gì
        /// </summary>
        protected IActionResult NoContent(string message = null){
            return new NoContentResult();
        } 
    }

    
}