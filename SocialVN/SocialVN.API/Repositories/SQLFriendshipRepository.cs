using Azure.Core;
using Microsoft.EntityFrameworkCore;
using SocialVN.API.Data;
using SocialVN.API.Enums;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public class SQLFriendshipRepository: GenericRepository<Friendship>, IFriendshipRepository
    {
        public SQLFriendshipRepository(SocialVNDbContext dbContext):base(dbContext)
        {
           
        }
   
        //Send a friend request
        //public async Task<Friendship> SendRequestAsync(Friendship friendship)
        //{
        //    await CreateAsync(friendship);
        //    return friendship;
        //}


        public async Task<Friendship> AcceptRequestAsync(Guid requestId)
        {
            var friendship = await dbContext.Friendships.FindAsync(requestId);
            if (friendship != null)
            {
                friendship.StatusEnum = FriendshipStatus.Accepted;
                friendship.UpdatedAt = DateTime.Now;
                await SaveAsync();
            }
            return friendship;
        }

        public async Task<Friendship> CancelRequestAsync(Guid requestId)
        {
            var friendship = await dbContext.Friendships
                .FirstOrDefaultAsync(f => f.Id == requestId && f.Status == FriendshipStatus.Pending.ToString());
            if (friendship != null)
            {
                dbContext.Friendships.Remove(friendship);
                await SaveAsync();
            }
            return friendship;
        }



        public async Task<FriendshipStatus> CheckFriendshipStatusAsync(string userId, string friendId)
        {
            var friendship = await dbContext.Friendships
                .FirstOrDefaultAsync(f => (f.RequesterId == userId && f.ReceiverId == friendId.ToString()) ||
                                          (f.RequesterId == friendId && f.ReceiverId == userId.ToString()));
            return friendship?.StatusEnum ?? FriendshipStatus.NotFriends;
        }


        public async Task<List<Friendship>> ListFriendRequestsAsync(Guid userId)
        {
            string userIdStr = userId.ToString();
            return await dbContext.Friendships
                .Include(f => f.Requester)
                .Include(f => f.Receiver)
                .Where(f => f.ReceiverId == userIdStr && f.Status == FriendshipStatus.Pending.ToString() &&
             f.RequesterId != userIdStr)
                .ToListAsync();
        }

     
        public async Task<List<ApplicationUser>> ListFriendsAsync(Guid userId)
        {
           return await dbContext.Friendships
                .Include(f => f.Requester)
                .Include(f => f.Receiver)
                .Where(f => (f.RequesterId == userId.ToString() || f.ReceiverId == userId.ToString()) && f.Status == FriendshipStatus.Accepted.ToString())
                .Select(f => f.RequesterId == userId.ToString() ? f.Receiver : f.Requester)
                .ToListAsync();
        }

        
        public async Task<Friendship> RejectRequestAsync(Guid requestId)
        {
            var friendship = await dbContext.Friendships
              .FirstOrDefaultAsync(f => f.Id == requestId && f.Status == FriendshipStatus.Pending.ToString());
            if (friendship != null)
            {
                friendship.StatusEnum = FriendshipStatus.Rejected;
                friendship.UpdatedAt = DateTime.Now;
                await SaveAsync();
            }
            return friendship;
        }
   
        public async Task<Friendship> RemoveFriendAsync(Guid friendId)
        {
            var friendship = await dbContext.Friendships
                .FirstOrDefaultAsync(f => f.Id == friendId && f.Status == FriendshipStatus.Accepted.ToString());
            if (friendship != null)
            {
                dbContext.Friendships.Remove(friendship);
                await SaveAsync();
            }
            
            return friendship;
        }
        public async Task<bool> IsFriendRequestExistsAsync(string userId, string receiverId)
        {
            return await dbContext.Friendships.AnyAsync(f =>
                ((f.RequesterId == userId && f.ReceiverId == receiverId) ||
                 (f.RequesterId == receiverId && f.ReceiverId == userId)) &&
                f.Status == FriendshipStatus.Pending.ToString());
        }
        public async Task<IEnumerable<ApplicationUser>> GetNewFriendsInLastWeek(string userId)
        {
            var lastWeek = DateTime.Now.AddDays(-7);

            var friendships = await dbContext.Friendships
                .Where(f => (f.RequesterId == userId || f.ReceiverId == userId) &&
                            f.Status == FriendshipStatus.Accepted.ToString() && 
                            f.CreatedAt >= lastWeek)
                .Select(f => f.RequesterId == userId ? f.Receiver : f.Requester) 
                .ToListAsync();

            return friendships;
        }


    }

}
