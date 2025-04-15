using System.Net;
using System.Text.Json;
using E_commerce.Api.Model;
using E_commerce.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;

namespace E_commerce.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        #region ===[Private Property]====
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region ===[Constructor]===
        public ExceptionHandlingMiddleware(
            RequestDelegate next, 
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment env
        ){
            _next = next;
            _logger = logger;
            _env = env;
        }
        #endregion

        #region =====[public methobs]=======
        
        //Xử lý các lỗi không mong muốn
        public async Task InvokeAsync(HttpContext context){
            try{
                await _next(context);

                //Xử lý trong trường hợp 404 - NotFound (Không có middleware nào xử lý)
                if(context.Response.StatusCode == (int)HttpStatusCode.NotFound || !context.Response.HasStarted){
                    context.Response.ContentType = "application/json";
                    var res = new ApiResponse<object>{
                        Success = false,
                        Message = "Không tìm thấy tài nguyên yêu cầu",
                        Error = new ErrorDetails{
                            Code = "RESOURCE_NOT_FOUND",
                            Details = "Không tìm thấy tài nguyên yêu cầu tại địa chỉ này"
                        },
                        Meta = new MetaData{
                            StatusCode = context.Response.StatusCode,
                            RequestId = context.TraceIdentifier,
                            Timestamp = DateTime.UtcNow
                        }
                    };
                    await context.Response.WriteAsJsonAsync(res);
                }
                
            }catch(Exception ex){
                await HandleExceptionAsync(context, ex);
            }
        }
        #endregion

        #region =========[Private method=========

        //Tạo thông báo lỗi trả về cho client
        private ApiResponse<object> CreateErrorResponse(
            HttpContext context, string message,
            string errorCode, string errorDetail
        ){
            return new ApiResponse<object>{
                Success = false,
                Message = message,
                Result = null,
                Error = new ErrorDetails{
                    Code = errorCode,
                    Details = errorDetail,
                },
                Meta = new MetaData{
                    StatusCode = context.Response.StatusCode,
                    RequestId = context.TraceIdentifier,
                    Timestamp = DateTime.UtcNow
                }
            };
        }

        // Hàm này để ghi log dựa trên res từ client.
        private void Log(Exception ex, LogLevel logLevel, HttpContext context){

            //Chuẩn bị thông tin log
            var logMessage = new {
                RequestId = context.TraceIdentifier,
                RequestPath = context.Request.Path,
                RequestMethod = context.Request.Method,
                StatusCode = context.Response.StatusCode,
                ErrorMessage = ex.Message,
                ErrorType = ex.GetType().Name,
                StackTrace = _env.IsDevelopment() ? ex.StackTrace : null,
                Timestamp = DateTime.UtcNow
            };

            //Chuyển đổi đối tượng log thành string cho logging
            string message = JsonSerializer.Serialize(logMessage);

            //Log theo mức độ phù hợp
            switch(logLevel)
            {
                case LogLevel.Error:
                    _logger.LogError(ex, message);
                    break;
                case LogLevel.Warning:
                    _logger.LogWarning(ex, message);
                    break;
                case LogLevel.Information:
                    _logger.LogInformation(ex, message);
                    break;
                default:
                    _logger.LogError(ex, message);
                    break;
            }
        }
        

        // Hiển thị thông báo lỗi 404 
        private async Task HandleNotFoundAsync(HttpContext context){
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            
            var res = CreateErrorResponse(
                context,
                "Không tìm thấy tài nguyên yêu cầu",
                "RESOURECE_NOT_FOUND",
                "Không tìm thấy tài nguyên yêu cầu tại địa chỉ này"
            );
            
            await context.Response.WriteAsJsonAsync(res);
        }


        private async Task HandleExceptionAsync(HttpContext context, Exception ex){
           
           context.Response.ContentType = "application/json";

           var res = new ApiResponse<object>{
                Success = false,
                Meta = new MetaData{
                    StatusCode = context.Response.StatusCode,
                    RequestId = context.TraceIdentifier,
                    Timestamp = DateTime.UtcNow
                }
           };

            //Xử lý các loại lỗi khác nhau
            switch(ex){
                case ECommerceException appEx:
                    //Xử lý exception từ framework
                    context.Response.StatusCode = appEx.StatusCode;
                    res.Message = appEx.Message;
                    res.Error = new ErrorDetails{
                        Code = appEx.ErrorCode,
                        Details = _env.IsDevelopment() ? appEx.StackTrace : appEx.Message
                    };
                    res.Meta.StatusCode = appEx.StatusCode;
                    _logger.LogWarning(ex, $"Application error: {appEx.Message}"); 
                    break;

                case MySqlException sqlEx:
                    //Xử lý exception từ MySQL
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    res.Message = "Lỗi kết nối đến cơ sở dữ liệu";
                    res.Error = new ErrorDetails{
                        Code = "DATABASE_ERROR",
                        Details = _env.IsDevelopment() ? sqlEx.Message : "Vui lòng thử lại sau"
                    };
                    res.Meta.StatusCode = 500;
                    _logger.LogWarning(ex, $"Database error: {sqlEx.Message}"); 
                    break;
                
                case TimeoutException:
                    //Xử lý lỗi timeout
                    context.Response.StatusCode = (int)HttpStatusCode.GatewayTimeout;
                    res.Message = "Yêu cầu đã hết thời gian chờ";
                    res.Error = new ErrorDetails{
                        Code = "TIMEOUT_ERROR",
                        Details = "Máy chủ không phản hồi trong thời gian cho phép"
                    };
                    res.Meta.StatusCode = 504;
                    _logger.LogWarning(ex, $"Time out error"); 
                    break;
                
                case ArgumentException argEx:
                    //Xử lý lỗi đối số
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    res.Message = argEx.Message;
                    res.Error = new ErrorDetails{
                        Code = "INVALID_ARGUMENT",
                        Details = argEx.Message
                    };
                    res.Meta.StatusCode = 400;
                    _logger.LogWarning(ex, $"Invalid argument: {argEx.Message}"); 
                    break;
                
                default:
                    //Xử lý các ngoại lệ không xác định khác
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    res.Message = "Đã xảy ra lỗi không xác định";
                    res.Error = new ErrorDetails{
                        Code = "INTERNAL_SERVER_ERROR",
                        Details = _env.IsDevelopment() ? ex.Message : "Vui lòng thử lại sau"
                    };
                    res.Meta.StatusCode = 500;
                    _logger.LogWarning(ex, $"Internal server error: {ex.Message}"); 
                    break;
            }

            await context.Response.WriteAsJsonAsync(res);
        }

        #endregion
    }

    //Extension method để sử dụng Middleware
    #region =====[Extension]=====
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
    #endregion
}