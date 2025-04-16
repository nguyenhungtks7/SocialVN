using SocialVN.API.Models.Domain;
using System;

namespace SocialVN.API.Repositories
{
    public interface ILikeRepository
    {

     
        Task<Like> CreateLikeAsync(Like like);
        Task<Like> DeleteLikeAsync(Guid id);
        Task<bool> IsLikeExistAsync(Guid postId, Guid userId);
        Task<int> CountLikesAsync(Guid postId);
        //Task<List<User>> GetUsersWhoLikedAsync(Guid postId);
    }
}
