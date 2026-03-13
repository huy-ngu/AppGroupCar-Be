using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using AppGroupCar.Helpers;

namespace AppGroupCar.Services.Mytoys.Photo
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

        public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),

                Folder = folder,

                Transformation = new Transformation()
                    .Height(500)
                    .Width(500)
                    .Crop("fill")
                    .Gravity("face")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            //if (uploadResult.Error != null)
            //    throw new Exception(uploadResult.Error.Message);

            return uploadResult;
        }


        public string GetPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            var uri = new Uri(url);

            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            // tìm vị trí upload
            var uploadIndex = Array.IndexOf(segments, "upload");

            if (uploadIndex == -1)
                return string.Empty;

            // bỏ qua upload + version
            var publicIdSegments = segments.Skip(uploadIndex + 2).ToList();

            if (!publicIdSegments.Any())
                return string.Empty;

            var lastSegment = publicIdSegments.Last();

            // bỏ extension
            publicIdSegments[publicIdSegments.Count - 1] =
                Path.GetFileNameWithoutExtension(lastSegment);

            return string.Join("/", publicIdSegments);
        }

    }
}
