namespace E_commerce.Application.Application
{
    public interface ILogger
    {
        //Log a message object
        void Debug(object message);
        void Info(object message);
        void Warn(object message);
        void Error(object message);
        void Fatal(object message);


        //Log a message object and exception
        void Debug(object message, Exception exception);
        void Info(object message, Exception exception);
        void Warn(object message, Exception exception);
        void Error(object message, Exception exception);
        void Fatal(object message, Exception exception);

        //Log an exception including the sack trace of exception
        void Debug(Exception exception);
        void Info(Exception exception);
        void Warn(Exception exception);
        void Error(Exception exception);
        void Fatal(Exception exception);
    }
}