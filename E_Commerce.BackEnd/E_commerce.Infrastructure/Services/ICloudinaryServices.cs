using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace E_commerce.Infrastructure.Services
{
    public interface ICloudinaryServices
    {
        ///<summary>
        /// Thêm ảnh
        /// </summary>
        public Task<ImageUploadResult> AddImageAssync(IFormFile file);

        /// <summary>
        /// Xóa ảnh dựa trên publicId
        /// </summary>
        public Task<DeletionResult> DeleteImageAssync(string publicId);

        /// <summary>
        /// thay đổi ảnh dựa trên public Id
        /// </summary>       
         public Task<ImageUploadResult> UpdateImageAssync(IFormFile file, string publicId);
    }
}