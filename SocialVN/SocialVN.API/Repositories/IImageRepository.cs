namespace SocialVN.API.Repositories
{
    public interface IImageRepository
    {
        Task<string> UploadAsync(IFormFile file);
        bool IsValidImage(IFormFile file);
        Task<string> UploadToImgurAsync(IFormFile file);
    }
}
