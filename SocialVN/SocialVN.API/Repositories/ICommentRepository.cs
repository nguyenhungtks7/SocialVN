using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<List<Comment>> GetAllCommentsAsync(string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 1000);

   
        Task<IEnumerable<Comment>> GetCommentsInLastWeek(string userId);
    }
}
