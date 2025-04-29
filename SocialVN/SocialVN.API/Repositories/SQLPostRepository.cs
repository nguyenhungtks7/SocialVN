using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialVN.API.Data;
using SocialVN.API.Enums;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;

namespace SocialVN.API.Repositories
{
    public class SQLPostRepository : GenericRepository<Post>, IPostRepository
    {
        public SQLPostRepository(SocialVNDbContext dbContext) : base(dbContext)
        { 
        }
        public async Task<List<Post>> GetTimelineAsync(string userId,int pageNumber = 1, int pageSize = 1000)
        {
             var friendIds = dbContext.Friendships
            .Where(f => f.RequesterId == userId || f.ReceiverId == userId)
            .AsEnumerable() 
            .Where(f => f.StatusEnum == FriendshipStatus.Accepted)
            .Select(f => f.RequesterId == userId ? f.ReceiverId : f.RequesterId)
            .ToList();

            return await dbContext.Posts
                .Where(p => friendIds.Contains(p.UserId))
                .OrderByDescending(p => p.CreatedAt)
                .Where(p =>   
                 p.Status == PostStatus.Public
                // Bài viết của chính mình
                || p.UserId == userId
                // Bài viết chỉ bạn bè xem được
                 || (p.Status == PostStatus.FriendsOnly && friendIds.Contains(p.UserId))
                )
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Include(p => p.User)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                 
                .ToListAsync();
        }

        public async Task<Post> GetPostByIdWithDetailsAsync(Guid id)
        {
            return await dbContext.Posts
                .Include(x => x.User)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(x => x.Id == id);
            
        }
        public async Task<IEnumerable<Post>> GetPostsCreatedInLastWeek(string userId)
        {
            var lastWeek = DateTime.Now.AddDays(-7);
            return await dbContext.Posts
                .Where(p => p.UserId == userId && p.CreatedAt >= lastWeek)
                .ToListAsync();
        }
    }
}
