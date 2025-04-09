using Microsoft.EntityFrameworkCore;
using SocialVN.API.Data;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public class SQLLikeRepository
    {
        private readonly SocialVNDbContext _context;
        public SQLLikeRepository(SocialVNDbContext context)
        {
            _context = context;
        }
        public async Task<Like> CreateLikeAsync(Like like)
        {
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
            return like;
        }
        public async Task<Like> DeleteLikeAsync(Guid id)
        {
            var like = await _context.Likes.FindAsync(id);
            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            return like;
        }
        //Check like exist
        public async Task<bool> IsLikeExistAsync(Guid postId, Guid userId)
        {
            return await _context.Likes.AnyAsync(l => l.PostId == postId && l.UserId == userId);
        }
        //Count likes
        public async Task<int> CountLikesAsync(Guid postId)
        {
            return await _context.Likes.CountAsync(l => l.PostId == postId);
        }
        //Get users who liked a post
        public async Task<List<User>> GetUsersWhoLikedAsync(Guid postId)
        {
            return await _context.Users
                .Where(u => u.Likes.Any(l => l.PostId == postId))
                .ToListAsync();
        }
    }
}
