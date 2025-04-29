using SocialVN.API.Models.Domain;
using System;

namespace SocialVN.API.Repositories
{
    public interface ILikeRepository : IGenericRepository<Like>
    {
        Task<bool> IsLikeExistAsync(Guid postId, Guid userId);
        Task<int> CountLikesAsync(Guid postId);

        Task<IEnumerable<Like>> GetLikesInLastWeek(string userId);
        Task<Like?> GetLikeByUserAndPostAsync(string userId, Guid postId);
    }
}
