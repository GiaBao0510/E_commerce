using E_commerce.Application.Application;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using E_commerce.Core.Exceptions;

namespace E_commerce.Infrastructure.Services.impl
{
    public class CloudinaryServices: ICloudinaryServices
    {
        #region  ===[Private Member]===
        private readonly Cloudinary _cloudinary;
        private readonly ILogger _logger;
        #endregion
        
        ///<summary>
        /// Hàm khởi tạo
        /// </summary>
        public CloudinaryServices(
            IConfiguration configuration,
            ILogger logger
        ){
            var account = new Account(
                configuration["Authentication:CloudinarySetting:CloudName"],
                configuration["Authentication:CloudinarySetting:ApiKey"],
                configuration["Authentication:CloudinarySetting:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
            _logger = logger;
        }

        ///<summary>
        /// Thêm ảnh
        /// </summary>
        public async Task<ImageUploadResult> AddImageAssync(IFormFile file)
        {
            //Nếu file null thì báo lỗi
            if(file.Length == 0){
                _logger.Error($"File rỗng hoặc không tồn tại. {nameof(file)}");
                throw new ValidationException($"File rỗng hoặc không tồn tại. {nameof(file)}");
            }

            try{
                var uploadResult = new ImageUploadResult();
                using (var stream = file.OpenReadStream()){                 //Mở luồng đọc file
                    var uploadParams = new ImageUploadParams(){             //Tham số upload
                        File = new FileDescription(file.FileName, stream),  //Đường dẫn file
                        Folder = "NguoiDung",                               //Thư mục lưu trữ trên cloudinary
                        Transformation = new Transformation()               // Chuyển đổi ảnh
                                        .Quality("auto")                    // Chất lượng ảnh tự động
                                        .FetchFormat("auto")                // Định dạng ảnh tự động
                                        .Flags("preserve_transparency")     // Giữ nguyên độ trong suốt
                    };

                    return await _cloudinary.UploadAsync(uploadParams);
                }
            }catch(Exception ex){
                _logger.Error($"Lỗi khi upload ảnh lên cloudinary: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Xóa ảnh dựa trên publicId
        /// </summary>
        public async Task<DeletionResult> DeleteImageAssync(string publicId){
            try
            {
                var deleteParams = new DeletionParams(publicId);
                return await _cloudinary.DestroyAsync(deleteParams);
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi khi xóa ảnh trên cloudinary: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// thay đổi ảnh dựa trên public Id
        /// </summary>       
        public async Task<ImageUploadResult> UpdateImageAssync(IFormFile file, string publicId){
            try{
                //Xóa ảnh cũ
                await DeleteImageAssync(publicId);

                //Cạp nhật lại ảnh mới
                return await AddImageAssync(file);
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi khi cập nhật ảnh trên cloudinary: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
    }
}