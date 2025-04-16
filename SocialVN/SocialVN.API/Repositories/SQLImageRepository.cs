namespace SocialVN.API.Repositories
{
    public class SQLImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment env;
        private readonly string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

        public SQLImageRepository(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(env.ContentRootPath, "Images");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/Images/{fileName}";
        }

        public bool IsValidImage(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(extension) && file.Length <= 10485760; 
        }
    }
}
