using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<Post> GetPostByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<Post>> GetPostsCreatedInLastWeek(string userId);
        Task<List<Post>> GetTimelineAsync(string userId, int pageNumber = 1, int pageSize = 1000);
    }
}
