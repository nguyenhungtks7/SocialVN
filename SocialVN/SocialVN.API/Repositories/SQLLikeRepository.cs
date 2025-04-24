using Microsoft.EntityFrameworkCore;
using SocialVN.API.Data;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public class SQLLikeRepository : ILikeRepository
    {
        private readonly SocialVNDbContext dbContext;
        public SQLLikeRepository(SocialVNDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Like> CreateLikeAsync(Like like)
        {
            await dbContext.Likes.AddAsync(like);
            await dbContext.SaveChangesAsync();
            return like;
        }
        public async Task<Like> DeleteLikeAsync(Guid id)
        {
            var like = await dbContext.Likes.FindAsync(id);
            if (like != null)
            {
                dbContext.Likes.Remove(like);
                await dbContext.SaveChangesAsync();
            }
            return like;
        }
        //Check like exist
        public async Task<bool> IsLikeExistAsync(Guid postId, Guid userId)
        {
            return await dbContext.Likes.AnyAsync(l => l.PostId == postId && l.UserId == userId.ToString());
        }
        //Count likes
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
