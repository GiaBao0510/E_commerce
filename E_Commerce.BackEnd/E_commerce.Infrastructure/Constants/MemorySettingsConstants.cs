namespace E_commerce.Infrastructure.Constants
{
    public static class MemorySettingsConstants
    {
        public const int BATCH_SIZE = 1000; // Số lượng bản ghi trong mỗi lô xử lý
        public const int MEMORY_THRESHOLD = 50 * 1024 * 1024; // Ngưỡng bộ nhớ tối đa cho phép (50MB)
        public const int STREAM_BUFFER_SIZE = 8192; // Kích thước bộ đệm luồng (8KB)
        public const int CHUNK_SIZE = 10000; // Kích thước mỗi khối dữ liệu (10.000 bản ghi)
        public const int MAX_FILE_SIZE = 500 * 1024 * 1024; // Kích thước tệp tối đa cho phép (500MB)
        public const int MAX_QUEUE_LENGTH = 10000; // Độ dài hàng đợi tối đa cho các tác vụ xử lý dữ liệu

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