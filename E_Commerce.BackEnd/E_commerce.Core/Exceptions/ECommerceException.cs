using MySql.Data.MySqlClient;

namespace E_commerce.Core.Exceptions
{
    public abstract class ECommerceException: Exception
    {
        public int StatusCode {get; }
        public string ErrorCode {get; }

        protected ECommerceException(string message, int statusCode, string errorCode)
            :base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }

    public class ValidationException : ECommerceException{
        /// <summary>
        /// Lỗi xác thực không hợp lệ - 400 - VALIDATION ERROR
        /// </summary>
        public ValidationException(string message)
            
            :base(message, 400, "VALIDATION_ERROR"){}
    }

    public class InvalidOperationException : ECommerceException{
        /// <summary>
        /// Lỗi không hợp lệ - 400 - Bad Request
        /// </summary>
        public InvalidOperationException(string message)
            
            :base(message, 400, "OPERATION_FAILED"){}
    }

    public class AuthenticationException : ECommerceException{
        /// <summary>
        /// Lỗi xác thực - 401 - Unauthorized
        /// </summary>
        public AuthenticationException(string message)
            :base(message, 401, "AUTHENTICATION_ERROR"){}
    }
    
    public class AuthorizationException : ECommerceException{
        /// <summary>
        /// Lỗi không có quyền truy cập - 403 - Forbidden
        /// </summary>
        public AuthorizationException(string message)
            :base(message, 403, "AUTHORIZATION_ERROR"){}
    }
    
    public class ResourceNotFoundException : ECommerceException{
        /// <summary>
        /// Lỗi không tìm thấy tài nguyên - 404 - Not Found
        /// </summary>
        public ResourceNotFoundException(string message)
            :base(message, 404, "NOT_FOUND"){}
    }

    
    public class ResourceConflictException : ECommerceException{
        /// <summary>
        /// Lỗi trùng lặp tài nguyên - 409 - Conflict
        /// </summary>
        public ResourceConflictException(string message)
            :base(message, 409, "RESOURCE_CONFLICT"){}
    }

    
    public class TooManyRequestsException : ECommerceException{
        /// <summary>
        /// Lỗi quá nhiều yêu cầu - 429 - Too Many Requests
        /// </summary>
        public TooManyRequestsException(string message)
            :base(message, 429, "TOO_MANY_REQUESTS"){}
    }

    public class DatabaseException : ECommerceException{
        /// <summary>
        /// Lỗi không xác định - 500 - Internal Server Error
        /// </summary>
        public DatabaseException(string message, Exception innerException = null)
            :base(message, 500, "DATABASE_ERROR"){}
    }
    
    public class ExternalServiceException : ECommerceException{
        /// <summary>
        /// Lỗi từ dịch vụ bên ngoài - 502- External Service Exception
        /// </summary>
        public ExternalServiceException(string serviceName, string message)
            :base($"Lỗi từ dịch vụ ngoài {serviceName}: {message}", 502, "EXTERNAL_SERVICE_ERROR"){}
    }

    public class DetailsOfTheException : ECommerceException{
        /// <summary>
        /// thông tin chi tiết về ngoại lệ - 500 - Details Of The Exception
        /// </summary>
        public DetailsOfTheException(Exception ex, string message = ".") 
            :base($"Thông tin chi tiết lỗi: {message}"+ 
                    $"[Message]: {ex.Message}"+
                    $"[Data]: {ex.Data}"+
                    $"[StackTrace]: {ex.StackTrace}"+
                    $"[TargetSite]: {ex.TargetSite}"+
                    $"[Source]: {ex.Source}", 500, "DETTAILS_OF_THE_EXCEPTION"){}
    }

    public class DetailsOfTheMysqlException : ECommerceException{
        /// <summary>
        /// thông tin chi tiết về ngoại lệ liên quan đến MYSQL- 500 - Details Of The Exception MySQL
        /// </summary>
        public DetailsOfTheMysqlException(MySqlException ex, string message = "Thông tin chi tiết lỗi MySQL")
            :base(
                message + ", Thông tin chi tiết:"+ 
                $"[Message]: {ex.Message}"+
                $"[Data]: {ex.Data}"+
                $"[StackTrace: {ex.StackTrace}"+
                $"[Number]: {ex.Number}"+
                $"[Code]: {ex.Code}"+
                $"[BatchCommand]: {ex.BatchCommand}"+
                $"[Source]: {ex.Source}"+
                $"[SqlState]: {ex.SqlState}"+
                $"[TargetSite]: {ex.TargetSite}", 500, "DETTAILS_OF_THE_MYSQL_EXCEPTION"
            ){}
    }
}