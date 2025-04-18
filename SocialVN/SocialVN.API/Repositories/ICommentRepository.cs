using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetAllCommentsAsync(string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 1000);
        Task<Comment?> GetCommentByIdAsync(Guid id);
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<Comment> UpdateCommentAsync(Guid id, Comment comment);
        Task<Comment> DeleteCommentAsync(Guid id);
        Task<IEnumerable<Comment>> GetCommentsInLastWeek(string userId);
    }
}
