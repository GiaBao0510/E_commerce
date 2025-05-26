using E_commerce.Api.Model;
using E_commerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class FileProccessingController : BaseApiController
    {
        private readonly IFileDataProccessingServices _fileDataProccessingServices;

        public FileProccessingController(IFileDataProccessingServices fileDataProccessingServices)
        {
            _fileDataProccessingServices = fileDataProccessingServices;
        }

        [HttpPost("update-data-from-csv")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5_368_709_120)]       //5GB
        [RequestTimeout(7200)]                  //2hours
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertDataFromCsv([FromForm] IFormFile file, [FromQuery] string tableName)
        {
            var result = await _fileDataProccessingServices.InsertDataFromCsv(tableName, file);
            return Success(result, "Cập nhật dữ liệu từ file .csv thành công");
        }

        [HttpPost("update-data-from-tsv")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5_368_709_120)]       //5GB
        [RequestTimeout(7200)]                  //2hours
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertDataFromTsv([FromForm] IFormFile file, [FromQuery] string tableName)
        {
            var result = await _fileDataProccessingServices.InsertDataFromTsv(tableName, file);
            return Success(result, "Cập nhật dữ liệu từ file .tsv thành công");
        }

    }
}