//using Microsoft.EntityFrameworkCore;
//using SocialVN.API.Data;
//using SocialVN.API.Models.Domain;

//namespace SocialVN.API.Repositories
//{
//    public class SQLUserRepository : IUserRepository
//    {
//        private readonly SocialVNAuthDbContext dbContext;

//        public SQLUserRepository(SocialVNAuthDbContext dbContext)
//        {
//            this.dbContext = dbContext;
//        }

//        public async Task<List<ApplicationUser>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
//        {
//            var user = dbContext.Users.AsQueryable();
//            //Filtering
//            if (string.IsNullOrEmpty(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
//            {
//                if (filterOn.Equals("FullName", StringComparison.OrdinalIgnoreCase))
//                {
//                    user = user.Where(x => x.FullName.Contains(filterQuery));
//                }
            
//            }
//            //Sorting 
//            if (string.IsNullOrEmpty(sortBy) ==false)
//            {
//                if(sortBy.Equals("FullName", StringComparison.OrdinalIgnoreCase))
//                {
//                    user = isAscending ? user.OrderBy(x=>x.FullName): user.OrderByDescending(x=>x.FullName);
//                }
//            }
//            //Pagination
//            var skipResults = (pageNumber -1)*pageSize;
//            return await user.Skip(skipResults).Take(pageSize).ToListAsync();
//        }
//        public async Task<User> CreateAsync(ApplicationUser user)
//        {
//           await dbContext.Users.AddAsync(user);
//            await dbContext.SaveChangesAsync();
//            return user;
//        }

//        public async Task<User> GetByIdAsync(Guid id)
//        {
//           return await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
//        }

//        public async Task<User> UpdateAsync(User user)
//        {
//            var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
//            if (existingUser == null)
//            {
//                return null;
//            }
//            existingUser.FullName = user.FullName;
//            existingUser.Email = user.Email;
//            existingUser.Avatar = user.Avatar;
//            existingUser.Role = user.Role;
//            existingUser.BirthDate = user.BirthDate;
//            existingUser.Occupation = user.Occupation;
//            existingUser.Location = user.Location;
//            existingUser.UpdatedAt = DateTime.Now;
//            await dbContext.SaveChangesAsync();
//            return existingUser;
//        }

//        public async Task<User> DeleteAsync(Guid id)
//        {
//           var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
//            if (existingUser == null)
//            {
//                return null;
//            }
//            dbContext.Users.Remove(existingUser);
//            dbContext.SaveChangesAsync();
//            return existingUser;
//        }
//    }
//}
