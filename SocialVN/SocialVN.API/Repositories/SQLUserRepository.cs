using Microsoft.EntityFrameworkCore;
using SocialVN.API.Data;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public class SQLUserRepository : IUserRepository
    {
        private readonly SocialVNDbContext dbContext;

        public SQLUserRepository(SocialVNDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<User>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            var user = dbContext.Users.AsQueryable();
            //Filtering
            if (string.IsNullOrEmpty(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("FullName", StringComparison.OrdinalIgnoreCase))
                {
                    user = user.Where(x => x.FullName.Contains(filterQuery));
                }
            
            }
            //Sorting 
            if (string.IsNullOrEmpty(sortBy) ==false)
            {
                if(sortBy.Equals("FullName", StringComparison.OrdinalIgnoreCase))
                {
                    user = isAscending ? user.OrderBy(x=>x.FullName): user.OrderByDescending(x=>x.FullName);
                }
            }
            //Pagination
            var skipResults = (pageNumber -1)*pageSize;
            return await user.Skip(skipResults).Take(pageSize).ToListAsync();
        }
        public async Task<User> CreteAsync(User user)
        {
           await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

    }
}
