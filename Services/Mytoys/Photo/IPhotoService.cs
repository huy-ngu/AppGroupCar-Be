using CloudinaryDotNet.Actions;

namespace AppGroupCar.Services.Mytoys.Photo
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
        Task<ImageUploadResult> UploadPhotoAsync(IFormFile file, string folder);
        string GetPublicIdFromUrl(string url);
    }
}
