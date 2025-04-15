using System.Reflection;
using E_commerce.Application.Application;
using log4net;

namespace E_commerce.Logging
{
    /* 
        - Lớp Logger triển khai mẫu thiết kế Singleton kết hợp với Facade(Một mẫu thiết kế
        đơn giản hóa hệ thống phúc tạp) để cung cấp một cách dễ dàng để ghi log thông tin
        - Giúp ghi log một cách nhất quán trong toàn bộ ứng dụng
    */
    public sealed class Logger: ILogger
    {
        #region ==[Private members]==
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        
        //Lazy initialization: Mục đích là để tạo ra một đối tượng mà chỉ được khởi tạo khi nó được sử dụng.
        private static readonly Lazy<Logger> _instance = new Lazy<Logger>(() => new  Logger());

        private const string ExceptionName = "Exception";
        private const string InnerExceptionName = "Inner Exception";
        private const string ExceptionMessageWithoutInnerException = "{0}{1}: {2} Message: {3}{4} StackTrace: {5}";
        private const string ExceptionMessageWithInnerException = "{0}{1}{2}";

        #endregion

        #region ===[ Properties ] ===

        //Gets the Logger instance
        public static Logger Instance{
            get{return _instance.Value;}
        }
        #endregion

        #region ===[ ILogger member ] ===

            // Logs a message object with the log4net.Core.Level.Debug level.
            public void Debug(object message){
                if(_logger.IsDebugEnabled)
                    _logger.Debug(message);
            }

            // Logs a message object with the log4net.Core.Level.Info level.
            public void Info(object message){
                if(_logger.IsInfoEnabled)
                    _logger.Info(message);
            }

            // Logs a message object with the log4net.Core.Level.Warn level.
            public void Warn(object message){
                if(_logger.IsWarnEnabled)
                    _logger.Warn(message);
            }

            // ogs a message object with the log4net.Core.Level.Error level.
            public void Error(object message){
                if(_logger.IsErrorEnabled)
                    _logger.Error(message);
            }

            // Logs a message object with the log4net.Core.Level.Fatal level.
            public void Fatal(object message){
                if(_logger.IsFatalEnabled)
                    _logger.Fatal(message);
            }

            // Logs a message object with the log4net.Core.Level.Debug level including the exception.
            public void Debug(object message, Exception exception){
                if(_logger.IsDebugEnabled)
                    _logger.Debug(message, exception);
            }

            //Logs a message object with the log4net.Core.Level.Info level including the exception.
            public void Info(object message, Exception exception){
                if(_logger.IsInfoEnabled)
                    _logger.Info(message, exception);
            }

            // Logs a message object with the log4net.Core.Level.Warn level including the exception.
            public void Warn(object message, Exception exception){
                if(_logger.IsWarnEnabled)
                    _logger.Warn(message, exception);
            }

            // Logs a message object with the log4net.Core.Level.Error level including the exception.
            public void Error(object message, Exception exception){
                if(_logger.IsErrorEnabled)
                    _logger.Error(message, exception);
            }
            
            // Logs a message object with the log4net.Core.Level.Fatal level including the exception.
            public void Fatal(object message, Exception exception){
                if(_logger.IsFatalEnabled)
                    _logger.Fatal(message, exception);
            }

            // Log an exception with the log4.Cỏe.Level.Debug level including the stack trace of the System.Exception passed as a paramenter 
            public void Debug(Exception exception){
                _logger.Debug(SerializeException(exception, ExceptionName));
            }
            
            // Log an exception with the log4.Cỏe.Level.Info level including the stack trace of the System.Exception passed as a paramenter
            public void Info(Exception exception){
                _logger.Info(SerializeException(exception, ExceptionName));
            }

            // Log an exception with the log4.Cỏe.Level.Warn level including the stack trace of the System.Exception passed as a paramenter
            public void Warn(Exception exception){
                _logger.Warn(SerializeException(exception, ExceptionName));
            }

            // Log an exception with the log4.Cỏe.Level.Error level including the stack trace of the System.Exception passed as a paramenter
            public void Error(Exception exception){
                _logger.Error(SerializeException(exception, ExceptionName));
            }

            // Log an exception with the log4.Cỏe.Level.Fatal level including the stack trace of the System.Exception passed as a paramenter
            public void Fatal(Exception exception){
                _logger.Fatal(SerializeException(exception, ExceptionName));
            }
        #endregion

        #region ===[ Public methods ] ===

        //Serialize Exception to get the complete message and stack trace
        public static string SerializeException(Exception exception){
            return SerializeException(exception, string.Empty);
        }
        #endregion

        #region ===[Private Methods]===
            
            //Serialize Exception to get the complete message and stack trace
            /* Lớp này dùng để chuyển đổi một đối tượng Exception thành văn bản có định dạng dễ đọc
            ,bao gồm đầy đủ thông tin về lỗi. */
            private static string SerializeException(Exception ex, string exceptionMessage){
                var mesgAndStackTrace = string.Format(
                    ExceptionMessageWithoutInnerException, Environment.NewLine,
                    exceptionMessage, Environment.NewLine, 
                    ex.Message, Environment.NewLine, 
                    ex.StackTrace
                );

                if(ex.InnerException != null){
                    mesgAndStackTrace = string.Format(
                        ExceptionMessageWithInnerException, 
                        mesgAndStackTrace,
                        Environment.NewLine, 
                        SerializeException(ex.InnerException, InnerExceptionName)
                    );
                }
                return mesgAndStackTrace + Environment.NewLine;
            }

        #endregion
    }
}