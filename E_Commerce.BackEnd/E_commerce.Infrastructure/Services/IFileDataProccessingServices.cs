
using Microsoft.AspNetCore.Http;
using MySqlConnector;

namespace E_commerce.Infrastructure.Services
{
    public interface IFileDataProccessingServices
    {
        //Xử lý chèn dữ liệu từ tệp tin .csv vào bảng
        public Task<bool> InsertDataFromCsv(string tableName, IFormFile file);

        //Xử lý chèn dữ liệu từ tệp tin .tsv vào bảng
        public Task<bool> InsertDataFromTsv(string tableName, IFormFile file);

        //Xác thực bảng có tồn tại không
        public Task<bool> CheckTableExist(string tableName);

        //Xác thực đầu vào
        public void ValidateInput(IFormFile file, string tableName);

        // Xử lý tệp tin < 50MB
        public Task<bool> ProccessSmallFileWithBulkLoad(
            string table,
            IFormFile file,
            string fieldTerminator,
            char? quotationCharacter,
            string fileExtension = "csv"
        );

        // Xử lý tệp tin > 50MB
        public Task<bool> ProccessLargeFileStreaming(
            string table,
            IFormFile file,
            string fieldTerminator,
            char? quotationCharacter
        );

        //Xử lý từng Batch dữ liệu
        public Task<int> ProccessBatchData(
            MySqlConnection connection,
            MySqlTransaction transaction,
            string tableName,
            List<string> batchData,
            int batchNumber,
            string fieldTerminator,
            char? quotationCharacter = null
        );

        //Dọn dẹp bộ nhớ theo định kỳ
        public Task PerformMemoryCleanup(int batchCount);

        //Xóa tệp tin tạm thời
        public Task CleanupTempFile(string tempFilePath);

        //Tạo file lưu tệp tin tạm thời 
        public Task<string> CreateTempFile(IFormFile file, string fileExtension);
    }
}