using Azure.Core;
using Microsoft.EntityFrameworkCore;
using SocialVN.API.Data;
using SocialVN.API.Enums;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public class SQLFriendshipRepository: IFriendshipRepository
    {
        private readonly SocialVNDbContext dbContext;
        public SQLFriendshipRepository(SocialVNDbContext dbContext)
        {
            dbContext = dbContext;
        }
        public async Task<Friendship?> GetByIdAsync(Guid requestId)
        {
            return await dbContext.Friendships
                .Include(f => f.Requester)
                .Include(f => f.Receiver)
                .FirstOrDefaultAsync(f => f.Id == requestId);
        }
        //Send a friend request
        public async Task<Friendship> SendRequestAsync(Friendship friendship)
        {
            await dbContext.Friendships.AddAsync(friendship);
            await dbContext.SaveChangesAsync();
            return friendship;
        }

        //Accept a friend request
        public async Task<Friendship> AcceptRequestAsync(Guid requestId)
        {
            var friendship = await dbContext.Friendships.FindAsync(requestId);
            if (friendship != null)
            {
                friendship.StatusEnum = FriendshipStatus.Accepted;
                friendship.UpdatedAt = DateTime.Now;
                await dbContext.SaveChangesAsync();
            }
            return friendship;
        }

        //Cancel a friend request
        public async Task<Friendship> CancelRequestAsync(Guid requestId)
        {
            var friendship = await dbContext.Friendships
                .FirstOrDefaultAsync(f => f.Id == requestId && f.StatusEnum == FriendshipStatus.Pending);
            if (friendship != null)
            {
                dbContext.Friendships.Remove(friendship);
                await dbContext.SaveChangesAsync();
            }
            return friendship;
        }


        //Check if a user is a friend
        public async Task<FriendshipStatus> CheckFriendshipStatusAsync(string userId, string friendId)
        {
            var friendship = await dbContext.Friendships
                .FirstOrDefaultAsync(f => (f.RequesterId == userId && f.ReceiverId == friendId.ToString()) ||
                                          (f.RequesterId == friendId && f.ReceiverId == userId.ToString()));
            return friendship?.StatusEnum ?? FriendshipStatus.NotFriends;
        }

        //Get a list of friend requests
        public async Task<List<Friendship>> ListFriendRequestsAsync(Guid userId)
        {
           return await dbContext.Friendships
                .Include(f => f.Requester)
                .Include(f => f.Receiver)
                .Where(f => f.ReceiverId == userId.ToString() && f.StatusEnum == FriendshipStatus.Pending)
                .ToListAsync();
        }

        //Get a list of friends
        public async Task<List<ApplicationUser>> ListFriendsAsync(Guid userId)
        {
           return await dbContext.Friendships
                .Include(f => f.Requester)
                .Include(f => f.Receiver)
                .Where(f => (f.RequesterId == userId.ToString() || f.ReceiverId == userId.ToString()) && f.StatusEnum == FriendshipStatus.Accepted)
                .Select(f => f.RequesterId == userId.ToString() ? f.Receiver : f.Requester)
                .ToListAsync();
        }

        //Reject a friend request
        public async Task<Friendship> RejectRequestAsync(Guid requestId)
        {
            var friendship = await dbContext.Friendships
              .FirstOrDefaultAsync(f => f.Id == requestId && f.StatusEnum == FriendshipStatus.Pending);
            if (friendship != null)
            {
                friendship.StatusEnum = FriendshipStatus.Rejected;
                friendship.UpdatedAt = DateTime.Now; 
                await dbContext.SaveChangesAsync();
            }
            return friendship;
        }
        //Remove a friend
        public async Task<Friendship> RemoveFriendAsync(Guid friendId)
        {
            var friendship = await dbContext.Friendships
                .FirstOrDefaultAsync(f => f.Id == friendId && f.StatusEnum == FriendshipStatus.Accepted);
            if (friendship != null)
            {
                dbContext.Friendships.Remove(friendship);
                await dbContext.SaveChangesAsync();
            }
            
            return friendship;
        }


    }
 
}
