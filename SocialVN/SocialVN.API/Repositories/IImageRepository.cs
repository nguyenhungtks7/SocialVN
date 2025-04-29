namespace SocialVN.API.Repositories
{
    public interface IImageRepository
    {
        bool IsValidImage(IFormFile file);
        Task<string> UploadToImgurAsync(IFormFile file);
    }
}
