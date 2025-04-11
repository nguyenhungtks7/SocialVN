using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public interface IPostRepository
    {
        Task<Post > CreateAsync(Post post);
        Task<Post> UpdateAsync(Guid id,Post post);
        Task<Post> DeleteAsync(Guid id);
        Task<Post> GetByIdAsync(Guid id);
        Task<List<Post>> GetAllAsync();
    }
}
