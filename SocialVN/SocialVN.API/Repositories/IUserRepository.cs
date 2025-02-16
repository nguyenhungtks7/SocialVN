using SocialVN.API.Models;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
    string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
       Task<User> CreteAsync(User user);    

      
    }
}
