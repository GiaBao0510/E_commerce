namespace E_commerce.Infrastructure.Constants
{
    //Các tham số liên quan đến cấu hình bộ nhớ và xử lý dữ liệu trong ứng dụng
    public static class MemorySettingsConstants
    {
        public const int BATCH_SIZE = 25000; // Số lượng bản ghi trong mỗi lô xử lý
        public const int MEMORY_THRESHOLD = 30 * 1024 * 1024; // Ngưỡng bộ nhớ tối đa cho phép (30MB)
        public const int STREAM_BUFFER_SIZE = 64 * 1024; // Kích thước bộ đệm luồng (64KB)
        public const int CHUNK_SIZE = 10000; // Kích thước mỗi khối dữ liệu (10.000 bản ghi)
        public const int MAX_FILE_SIZE = 1 * 1024 * 1024 * 1024; // Kích thước tệp tối đa cho phép (1GB)
        public const int MAX_QUEUE_LENGTH = 10000; // Độ dài hàng đợi tối đa cho các tác vụ xử lý dữ liệu

        #region MEMORY_MANAGEMENT
        #endregion

        #region DATABASE_OPTIMIZATION
        #endregion

        #region PARALLEL_
        #endregion
        /// <summary>
        /// Số lần retry tối đa cho cleanup file
        /// </summary>
        public const int MAX_CLEANUP_ATTEMPTS = 5;

        /// <summary>
        /// Interval cho memory cleanup (mỗi 10 batches)
        /// </summary>
        public const int MEMORY_CLEANUP_INTERVAL = 10;
    }
}