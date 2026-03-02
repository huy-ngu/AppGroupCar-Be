using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Image;

namespace WebCarOrder.Services.Photo
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {


            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                // Mở luồng đọc file
                using var stream = file.OpenReadStream();

                // Cấu hình tham số upload
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),


                    Folder = "driver-avatars",

                    Transformation = new Transformation()
                    .Height(500).Width(500)
                    .Crop("fill").Gravity("face")
                };

                // Upload lên
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }

}
