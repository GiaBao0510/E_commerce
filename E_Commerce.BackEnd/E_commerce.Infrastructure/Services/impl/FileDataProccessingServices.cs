using E_commerce.Application.Application;
using Microsoft.Extensions.Configuration;
using E_commerce.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using E_commerce.SQL.Queries;
using E_commerce.Infrastructure.Constants;
using System.Text;
using MySqlConnector;

namespace E_commerce.Infrastructure.Services.impl
{
    public class FileDataProccessingServices: IFileDataProccessingServices
    {
        #region ====[Private fields]===
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        #endregion

        //Hàm khởi tạo
        public FileDataProccessingServices(
            ILogger logger,
            IConfiguration configuration
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Xử lý chèn dữ liệu từ tệp tin .csv vào bảng
        /// </summary>
        public async Task<bool> InsertDataFromCsv(string tableName, IFormFile file)
        {
            //Validation đầu vào
            ValidateInput(file, tableName);

            //Kiểm tra bảng có tồn tại không
            if (!await CheckTableExist(tableName))
               throw new ResourceNotFoundException($"Bảng {tableName} không tồn tại.");

            // Strategy_pattern: xử lý dự trên kích cỡ file
            if (file.Length > MemorySettingsConstants.MEMORY_THRESHOLD)
            {
                _logger.Info($"File lớn ({file.Length:N0} bytes) - sử dụng Streaming Processing");
                return await ProccessLargeFileStreaming(tableName, file, ",", '"');
            }
            else
            {
                _logger.Info($"File nhỏ ({file.Length:N0} bytes) - sử dụng Bulk Load truyền thống");
                return await ProccessSmallFileWithBulkLoad(tableName, file, ",", '"', "csv");
            }
        }

        /// <summary>
        /// Xử lý chèn dữ liệu từ tệp tin .tsv vào bảng
        /// </summary>
        public async Task<bool> InsertDataFromTsv(string tableName, IFormFile file)
        {
            //Validation đầu vào
            ValidateInput(file, tableName);

            //Kiểm tra bảng có tồn tại không
            if (!await CheckTableExist(tableName))
               throw new ResourceNotFoundException($"Bảng {tableName} không tồn tại.");

            // Strategy_pattern: xử lý dự trên kích cỡ file
            if (file.Length > MemorySettingsConstants.MEMORY_THRESHOLD)
            {
                _logger.Info($"File lớn ({file.Length:N0} bytes) - sử dụng Streaming Processing");
                return await ProccessLargeFileStreaming(tableName, file, "\t", null);
            }
            else
            {
                _logger.Info($"File nhỏ ({file.Length:N0} bytes) - sử dụng Bulk Load truyền thống");
                return await ProccessSmallFileWithBulkLoad(tableName, file, "\t", null, "tsv");
            }
        }

        /// <summary>
        /// Xác thực bảng có tồn tại không
        /// </summary>
        public async Task<bool> CheckTableExist(string tableName)
        {
            try
            {
                using var connection = new MySqlConnection(_configuration["Database:MySQL"]);
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = FileDataProccessingQueries.CheckTableExist;
                command.Parameters.Add(new MySqlParameter("@TABLE_NAME", tableName));
                var result = command.ExecuteScalar();

                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex) when (!(ex is ECommerceException))
            {
                _logger.Error($"Lỗi khi kiểm tra bảng tồn tại: {ex.Message}");
                throw new DetailsOfTheException(ex, "Lỗi khi kiểm tra bảng tồn tại");
            }
        }

        /// <summary>
        /// Xác thực đầu vào
        /// </summary>
        /// <param name="file"></param>
        /// <param name="tableName"></param>
        public void ValidateInput(IFormFile file, string tableName)
        {
            //Validation đầu vào
            if (file == null || file.Length == 0)
                throw new ValidationException("Tệp tin không hợp lệ hoặc không có dữ liệu.");
            
            if(string.IsNullOrEmpty(tableName))
                throw new ValidationException("Tên bảng không được để trống.");
        }

        /// <summary>
        /// Xử lý tệp tin (< 50MB) với Bulk Load truyền thống
        /// </summary>
        public async Task<bool> ProccessSmallFileWithBulkLoad(
            string table,
            IFormFile file,
            string fieldTerminator,
            char? quotationCharacter,
            string fileExtension = "csv"
        )
        {
            string tempFilePath = string.Empty;

            try
            {
                //Tạo tệp tin tạm thời
                tempFilePath = await CreateTempFile(file, fileExtension);

                using var connection = new MySqlConnection(_configuration["Database:MySQL"]);
                await connection.OpenAsync();
                using var transaction = await connection.BeginTransactionAsync();

                try
                {
                    var bulkLoader = new MySqlBulkLoader(connection)
                    {
                        TableName = table,
                        FieldTerminator = fieldTerminator,
                        LineTerminator = "\n",
                        EscapeCharacter = '\\',
                        FileName = tempFilePath,
                        Local = true
                    };

                    if (quotationCharacter.HasValue)
                        bulkLoader.FieldQuotationCharacter = quotationCharacter.Value;

                    var rowInsert = await bulkLoader.LoadAsync();
                    await transaction.CommitAsync();

                    _logger.Info($"Đã chèn {rowInsert} dòng dữ liệu vào bảng {table}.");
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.Error($"Lỗi khi chèn dữ liệu từ tệp tin {file.FileName} vào bảng {table}: {ex.Message}");
                    throw new DetailsOfTheException(ex, $"Lỗi khi chèn dữ liệu từ tệp tin {file.FileName} vào bảng {table}");
                }
            }
            finally
            {
                await CleanupTempFile(tempFilePath);
            }    
        }

        /// <summary>
        /// Xử lý tệp tin > 50MB
        /// </summary>
        public async Task<bool> ProccessLargeFileStreaming(
            string table,
            IFormFile file,
            string fieldTerminator,
            char? quotationCharacter
        )
        {
            using var connection = new MySqlConnection(_configuration["Database:MySQL"]);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                //Các biến hỗ trợ
                var totalProcessed = 0;
                var batchCount = 0;
                var currentBatch = new List<string>();

                //STREAMING: Đọc file theo từng dòng thay vò phải đọc hết
                using var reader = new StreamReader(
                    file.OpenReadStream(),
                    Encoding.UTF8,
                    bufferSize: MemorySettingsConstants.STREAM_BUFFER_SIZE
                );

                string line;
                var startTime = DateTime.UtcNow;

                // Xử lý đọc từng dòng thay vì phải LOAD toàn bộ File vào Memory
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;  // Bỏ qua dòng trống

                    //Thêm dòng vào batch hiện tại
                    currentBatch.Add(line);

                    //BATCH PROCCESSING: khi batch đủ lớn so với mức quy định thì sử lý
                    if (currentBatch.Count > MemorySettingsConstants.BATCH_SIZE)
                    {
                        var processed = await ProccessBatchData(
                            connection,
                            transaction,
                            table,
                            currentBatch,
                            batchCount,
                            fieldTerminator,
                            quotationCharacter
                        );

                        totalProcessed += processed;
                        batchCount++;

                        //MEMORY_MANAGEMENT: Xóa Batch đã xử lý
                        currentBatch.Clear();

                        //Ghi lại tiến trình
                        var elapsed = DateTime.UtcNow - startTime;
                        var avgSeed = totalProcessed / elapsed.TotalSeconds;
                        _logger.Info(
                            $"Đã xử lý {totalProcessed} dòng dữ liệu sau {elapsed.TotalSeconds:F2} giây. " +
                            $"\nTốc độ trung bình: {avgSeed:F2} dòng/giây. Batch {batchCount}."
                        );
                        
                        //MEMORY_MANAGEMENT: Cứ mỗi 10 batch thì dọn dẹp memory
                        if(batchCount % MemorySettingsConstants.MEMORY_CLEANUP_INTERVAL == 0)
                            await PerformMemoryCleanup(batchCount); 
                    }
                }

                //Xử lý nếu Batch cuối cũng còn sót
                if (currentBatch.Count > 0)
                {
                    var processed = await ProccessBatchData(
                        connection,
                        transaction,
                        table,
                        currentBatch,
                        batchCount,
                        fieldTerminator,
                        quotationCharacter
                    );

                    totalProcessed += processed;
                    _logger.Info($"Batch cuối {batchCount + 1}: {processed} records");
                }

                await transaction.CommitAsync();
                
                var totalTime = DateTime.UtcNow - startTime;
                var finalSpeed = totalProcessed / totalTime.TotalSeconds;
                _logger.Info($"HOÀN THÀNH: {totalProcessed:N0} records trong {totalTime:hh\\:mm\\:ss} | Tốc độ TB: {finalSpeed:F0} records/sec");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.Error($"Lỗi khi chèn dữ liệu lớn từ file: {ex.Message}");
                throw new DetailsOfTheException(ex, $"Lỗi khi chèn dữ liệu lớn từ file {file.FileName} vào bảng {table}");
            }
        }

        /// <summary>
        /// Xử lý từng Batch dữ liệu
        /// </summary>
        public async Task<int> ProccessBatchData(
            MySqlConnection connection,
            MySqlTransaction transaction,
            string tableName,
            List<string> batchData,
            int batchNumber,
            string fieldTerminator,
            char? quotationCharacter = null
        )
        {
            var tempFilePath = Path.GetTempFileName();

            try
            {
                //ghi batch vào tệp tin tạm thời
                await File.WriteAllLinesAsync(tempFilePath, batchData, Encoding.UTF8);

                //Sử dụng MySqlBulkLoader để chèn dữ liệu từ tệp tin tạm thời
                var bulkLoader = new MySqlBulkLoader(connection)
                {
                    TableName = tableName,
                    FieldTerminator = fieldTerminator,
                    LineTerminator = "\n",
                    FileName = tempFilePath,
                    EscapeCharacter = '\\',
                    Local = true
                };

                //Chỉ thêm quotation character nếu có
                if (quotationCharacter.HasValue)
                    bulkLoader.FieldQuotationCharacter = quotationCharacter.Value;

                //insert dữ liệu vào bảng
                var rowsInserted = await bulkLoader.LoadAsync();

                _logger.Info($"Batch {batchNumber + 1}: Đã chèn {rowsInserted} dòng dữ liệu vào bảng {tableName}.");
                return rowsInserted;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi khi xử lý Batch {batchNumber}: {ex.Message}");
                throw;
            }
            finally
            {
                // Dọn dẹp bộ nhớ sau khi xử lý xong batch
                await PerformMemoryCleanup(batchNumber);
            }
        }

        /// <summary>
        /// Dọn dẹp bộ nhớ theo định kỳ (Đồng thời ghi lại thông tin)
        /// </summary>
        public async Task PerformMemoryCleanup(int batchCount)
        {
            try
            {
                //Lấy thông tin bộ nhớ trước khi cleanup
                var memoryBefore = GC.GetTotalMemory(false);    

                //Giải phóng bộ nhớ không còn sử dụng
                GC.Collect();                   // Cưỡng bức thu gom rác
                GC.WaitForPendingFinalizers();  // Đợi cho các đối tượng đang được giải phóng hoàn tất
                GC.Collect();                   // Cưỡng bức thu gom rác lần nữa để đảm bảo tất cả bộ nhớ đã được giải phóng

                // Đợi cho hệ thống ổn định lại
                await Task.Delay(200); // 0.2s

                //Lấy thông tin bộ nhớ sau khi cleanup
                var memoryAfter = GC.GetTotalMemory(true);      // Lấy bộ nhớ sau khi dọn dẹp
                _logger.Info(
                    $"Memoruy cleanup sau khi batch {batchCount}: " +
                    $"Trước: {memoryBefore / 1024 / 1024:F1} MB"+
                    $", Sau: {memoryAfter / 1024 / 1024:F1} MB" 
                );
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi khi dọn dẹp bộ nhớ: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa tệp tin tạm thời
        /// </summary>
        public async Task CleanupTempFile(string tempFilePath)
        {
            if (string.IsNullOrEmpty(tempFilePath) || !File.Exists(tempFilePath))
                return;
            
            var attempts = 0;

            while (attempts < MemorySettingsConstants.MAX_CLEANUP_ATTEMPTS)
            {
                try
                {
                    await Task.Delay(100 * (attempts + 1));
                    File.Delete(tempFilePath);
                    return; // Thoát khỏi vòng lặp nếu xóa thành công
                }
                catch (Exception ex)
                {
                    attempts++;
                    if (attempts >= MemorySettingsConstants.MAX_CLEANUP_ATTEMPTS)
                        _logger.Error($"Không thể xóa tệp tin tạm thời {tempFilePath} sau {MemorySettingsConstants.MAX_CLEANUP_ATTEMPTS} lần thử: {ex.Message}");   
                }
            } 
        }

        /// <summary>
        /// Tạo file lưu tệp tin tạm thời 
        /// </summary>
        public async Task<string> CreateTempFile(IFormFile file, string fileExtension)
        {
            var tempDirectory = Path.GetTempPath();
            var fileName = $"temp_import_{Guid.NewGuid()}.{fileExtension}";
            var tempFilePath = Path.Combine(tempDirectory, fileName);

            using var sourceStream = file.OpenReadStream();
            using var targetStream = new FileStream(
                tempFilePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: MemorySettingsConstants.STREAM_BUFFER_SIZE,
                useAsync: true
            );

            await sourceStream.CopyToAsync(targetStream);
            return tempFilePath;
        }
    }
}