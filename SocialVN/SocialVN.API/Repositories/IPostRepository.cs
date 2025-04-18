using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public interface IPostRepository
    {
        Task<Post > CreateAsync(Post post);
        Task<Post> UpdateAsync(Guid id,Post post);
        Task<Post> DeleteAsync(Guid id);
        Task<Post> GetByIdAsync(Guid id);
        Task<IEnumerable<Post>> GetPostsCreatedInLastWeek(string userId);
        Task<List<Post>> GetTimelineAsync(string userId, int pageNumber = 1, int pageSize = 1000);
    }
}
