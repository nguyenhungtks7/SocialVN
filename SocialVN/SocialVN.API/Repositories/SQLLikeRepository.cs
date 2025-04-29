using Microsoft.EntityFrameworkCore;
using SocialVN.API.Data;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public class SQLLikeRepository : GenericRepository<Like>, ILikeRepository
    {
        public SQLLikeRepository(SocialVNDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<bool> IsLikeExistAsync(Guid postId, Guid userId)
        {
            return await dbContext.Likes.AnyAsync(l => l.PostId == postId && l.UserId == userId.ToString());
        }
        public async Task<int> CountLikesAsync(Guid postId)
        {
            return await dbContext.Likes.CountAsync(l => l.PostId == postId);
        }
        //Get users who liked a post
        //public async Task<List<User>> GetUsersWhoLikedAsync(Guid postId)
        //{
        //    return await dbContext.Users
        //        .Where(u => u.Likes.Any(l => l.PostId == postId))
        //        .ToListAsync();
        //}
        public async Task<IEnumerable<Like>> GetLikesInLastWeek(string userId)
        {
            var lastWeek = DateTime.Now.AddDays(-7);
            return await dbContext.Likes
                .Where(l => l.UserId == userId && l.CreatedAt >= lastWeek)
                .ToListAsync();
        }
        public async Task<Like?> GetLikeByUserAndPostAsync(string userId, Guid postId)
        {
            return await dbContext.Likes
                .FirstOrDefaultAsync(like => like.UserId == userId && like.PostId == postId);
        }
    }
}
