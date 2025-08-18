using BusinessLogicLayer.Utilities.Cloudinary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface ICloudinaryService
    {
        Task<CloudinaryUploadResult> UploadFileAsync(Stream fileStream, string fileName, string contentType);
        Task<string> UploadAvatarAsync(Stream fileStream, string fileName);
        Task<bool> DeleteFileAsync(string publicId, string resourceType);
    }
}
