using System.Net;
using Microsoft.AspNetCore.Authorization;
using E_commerce.Infrastructure.Services;
using E_commerce.Api.Model;
using E_commerce.Core.Exceptions;

namespace E_commerce.Api.Authentication
{
    /// <summary>
    /// Mục tiêu của lớp này dùng để kiểm tra xem token người dùng đưa vào liệu có trong whitelist
    /// </summary>
    public class ValidateTokenInsideWhiteList
    {
        #region  =======[private elements]=======
        private readonly RequestDelegate _next;
        private readonly Application.Application.ILogger _logger;
        #endregion

        ///<summary>
        /// Hàm khởi tạo
        /// </summary>
        public ValidateTokenInsideWhiteList(
            RequestDelegate next,
            Application.Application.ILogger logger
        ){
            _next = next;
            _logger = logger;
        }

        ///<summary>
        /// Hàm lấy token từ request
        /// .Thứ tự lấy ưu theo: 1.Authorization header  2.Cookie 3.Querry
        /// </summary>
        private string GetTokenFromRequest(HttpContext context){

            //1. Kiểm tra trong Authorization header - Nếu có tìm thấy thì sẽ trả về
            string authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if(
                !string.IsNullOrEmpty(authHeader) &&
                authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            )
                return authHeader.Substring(7).Trim();
            

            //2. Kiểm tra trong Cookie - Nếu thấy thì sẽ trả về
            if(context.Request.Cookies.TryGetValue("access_token", out string cookieToken)){
                _logger.Info($"Token trong cookie: {cookieToken}");
                return cookieToken;
            }
                
            
            //3. Kiểm tra trong Querry string (cho WebSocket hoặc SSE)
            if(context.Request.Query.TryGetValue("access_token", out var querryToken)){
                _logger.Info($"Token trong querry: {querryToken}");
                return querryToken.ToString();
            }

            return null; //Không tìm thấy token trong request
        }

        ///<summary>
        /// Hàm kiểm tra xem token có trong whiteLisst hay không
        /// </summary>
        public async Task InvokeAsync(HttpContext context){

            //1. Kiểm tra xem endpoint có yêu cầu xác thực không
            var endPoint = context.GetEndpoint();
            _logger.Info($"Endpoint: {endPoint?.DisplayName}, AllowAnonymous: {endPoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null}");
            _logger.Info($"Context -- PATH:{context.Request.Path}; -- METHOB:{context.Request.Method}; -- HEADER: {context.Request.Headers}");

            //1.1 Bỏ qua neeys endpoint có attribute [AllowAnonymous]
            if(endPoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null || endPoint == null){
                await _next(context);
                return;
            }

            //1.2 Bỏ qua nếu endpoint không có attribute [Aithorize]
            var authorizationAttributes = endPoint?.Metadata?.GetOrderedMetadata<IAuthorizeData>();
            if(authorizationAttributes == null || !authorizationAttributes.Any()){
                await _next(context);
                return;
            }

            //1.3 Lấy token từ request
            string token = GetTokenFromRequest(context);
            _logger.Info($"[Token cần xét]: {token}");
            if(string.IsNullOrEmpty(token)){
                _logger.Warn("Không tìm thấy token trong request");
                await _next(context);
                return;
            }

            //Nếu thấy được token
            if(!string.IsNullOrEmpty(token)){
                //Kiểm tra token có trong whitelist hay không
                try{
                    var tokenListManagementService = context.RequestServices.GetRequiredService<ITokenListManagementService>();

                    //Kiểm tra token trong whitelist
                    var score = await tokenListManagementService.IsTokenInSortedSet(token);

                    //Nếu tìm thấy vẫn còn hạn thì cho phép truy cập
                    if(score > DateTimeOffset.UtcNow.ToUnixTimeSeconds()){
                        //Cho phép request tiếp tục
                        await _next(context);
                        return;
                    }

                    //Token hết hạn thì xóa token 
                    await tokenListManagementService.DeleteTokenFromSortedSet(token);
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsJsonAsync(new ApiResponse<object>{
                        Success = false,
                        Message = "Token đã hết hạn",
                        Error = new ErrorDetails{
                            Code = "TOKEN_EXPIRED",
                            Details = "token đã hết hạn"
                        },
                        Meta = new MetaData{
                            StatusCode = context.Response.StatusCode,
                            RequestId = context.TraceIdentifier,
                            Timestamp = DateTime.UtcNow
                        }
                    });
                    return;
                }
                catch(Exception ex) when (!(ex is ECommerceException)){
                    _logger.Error($"Lỗi khi kiểm tra token trong whitelist: {ex.Message}");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsJsonAsync(new ApiResponse<object>{
                        Success = false,
                        Message = "Lỗi khi kiểm tra token trong whitelist",
                        Error = new ErrorDetails{
                            Code = "INTERNAL_SERVER_ERROR",
                            Details = "Lỗi khi kiểm tra token trong whitelist"
                        },
                        Meta = new MetaData{
                            StatusCode = context.Response.StatusCode,
                            RequestId = context.TraceIdentifier,
                            Timestamp = DateTime.UtcNow
                        }
                    });

                    return;
                }
            }

            //Nếu không có token hay endpoint thì không yêu cầu xác thực
            await _next(context);
        }
    }
}