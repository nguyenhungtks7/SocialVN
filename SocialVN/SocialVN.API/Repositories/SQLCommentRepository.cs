using Microsoft.EntityFrameworkCore;
using SocialVN.API.Data;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public class SQLCommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public SQLCommentRepository(SocialVNDbContext dbContext):base(dbContext)
        {          
        }
        public async Task<List<Comment>> GetAllCommentsAsync(string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 1000)
        {
            var coments = dbContext.Comments.Include(x=>x.Post).Include(x=>x.User).AsQueryable();
            //Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if(sortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                {
                    coments = isAscending ? coments.OrderBy(x => x.CreatedAt) : coments.OrderByDescending(x => x.CreatedAt);
                }
            }
            //Pagination
            var skipResults = (pageNumber - 1) * pageSize;
            return await coments.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsInLastWeek(string userId)
        {
            var lastWeek = DateTime.Now.AddDays(-7);
            return await dbContext.Comments
                .Where(c => c.UserId == userId && c.CreatedAt >= lastWeek)
                .ToListAsync();
        }
    }
}
