using SocialVN.API.Enums;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public interface IFriendshipRepository
    {
        Task<Friendship?> GetByIdAsync(Guid requestId);
        Task<Friendship> SendRequestAsync(Friendship friendship);
        Task<Friendship> AcceptRequestAsync(Guid requestId);
        Task<Friendship> RejectRequestAsync(Guid requestId);
        Task<Friendship> CancelRequestAsync(Guid requestId);
        Task<Friendship> RemoveFriendAsync(Guid friendId);
        Task<List<ApplicationUser>> ListFriendsAsync(Guid friendId);
        Task<List<Friendship>> ListFriendRequestsAsync(Guid userId);
        Task<FriendshipStatus> CheckFriendshipStatusAsync(string userId, string friendId);
        Task<bool> IsFriendRequestExistsAsync(string userId, string receiverId);
        Task<IEnumerable<ApplicationUser>> GetNewFriendsInLastWeek(string userId);

    }
}
