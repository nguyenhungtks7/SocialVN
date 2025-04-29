using Newtonsoft.Json.Linq;
using SocialVN.API.Models.DTO;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SocialVN.API.Repositories
{
    public class SQLImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment env;
        private readonly string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        //private readonly string imgurClientId = "a3acf28f6286aba";
        private readonly string _clientId;
        public SQLImageRepository(IWebHostEnvironment env, IConfiguration config)
        {
            _clientId = config["ImgurClientId"];
        } 
        public bool IsValidImage(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(extension) && file.Length <= 10485760; 
        }
        public async Task<string> UploadToImgurAsync(IFormFile file)
        {

            const string url = "https://api.imgur.com/3/image";     // endpoint upload
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Client-ID " + _clientId);
            client.DefaultRequestHeaders.ExpectContinue = false;    // tắt Expect:100-continue

            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(streamContent, "image", file.FileName);

            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"Upload to Imgur failed: {response.StatusCode} – {err}");
            }

            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            return json["data"]?["link"]?.ToString()
                   ?? throw new Exception("Không lấy được link ảnh từ response.");
        }
    }
}
