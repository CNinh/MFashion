using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Utilities.Cloudinary;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySetting _setting;


        public CloudinaryService(IOptions<CloudinarySetting> setting)
        {
            _setting = setting.Value;

            Account account = new Account(
                _setting.CloudName,
                _setting.ApiKey,
                _setting.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<CloudinaryUploadResult> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            bool isImage = !string.IsNullOrEmpty(contentType) &&
                            contentType.StartsWith("image", StringComparison.OrdinalIgnoreCase);

            UploadResult result;
            string resourceType;

            if (isImage)
            {
                resourceType = "image";

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, fileStream),
                    Folder = _setting.ImageFolder
                };

                result = await _cloudinary.UploadAsync(uploadParams);
            }
            else
            {
                resourceType = "raw";

                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(fileName, fileStream),
                    Folder = _setting.OtherFolder,
                };
                
                result = await _cloudinary.UploadAsync(uploadParams);
            }

            return new CloudinaryUploadResult
            {
                Url = result.SecureUrl.ToString(),
                PublicId = result.PublicId,
                ResourceType = resourceType
            };
        }

        public async Task<string> UploadAvatarAsync(Stream fileStream, string fileName)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = _setting.ImageFolder,
                Transformation = new Transformation().Width(150).Height(150).Crop("fill")
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }

        public async Task<bool> DeleteFileAsync(string publicId, string resourceType)
        {
            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = resourceType switch
                {
                    "image" => ResourceType.Image,
                    "raw" => ResourceType.Raw,
                    _ => ResourceType.Auto
                }
            };

            var result = await _cloudinary.DestroyAsync(deletionParams);
            return result.Result == "ok";
        }
    }
}
