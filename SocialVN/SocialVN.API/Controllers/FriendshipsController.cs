using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialVN.API.Controllers
{
    // https:localhost:portnumber/api/friendships
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipsController : ControllerBase
    {
        private readonly IFriendshipRepository friendshipRepository;
        private readonly IMapper mapper;
        public FriendshipsController(IFriendshipRepository friendshipRepository, IMapper mapper)
        {
            this.friendshipRepository = friendshipRepository;
            this.mapper = mapper;
        }

        // Send friend request
        // POST: http:localhost:portnumber/api/friendships
        [SwaggerOperation (Summary = "Send friend request", Description = "Gửi yêu cầu kết bạn.")]
        [HttpPost]
        public async Task<IActionResult> SendFriendRequest([FromBody] AddFriendshipRequestDto friendship)
        {
            var friendshipDomainModel = mapper.Map<Friendship>(friendship);
            await friendshipRepository.SendRequestAsync(friendshipDomainModel);
            return Ok(mapper.Map<FriendshipDto>(friendshipDomainModel));
        }

        // Accept friend request
        // PUT: http://localhost:portnumber/api/friendships/accept/{requestId}
        [SwaggerOperation(Summary = "Accept friend request", Description = "Chấp nhận yêu cầu kết bạn.")]
        [HttpPut("accept/{requestId:Guid}")]
        public async Task<IActionResult> AcceptFriendRequest(Guid requestId)
        {
            var friendshipDomainModel = await friendshipRepository.AcceptRequestAsync(requestId);
            return Ok(mapper.Map<FriendshipDto>(friendshipDomainModel));
        }

        //Cancel friend request
        // DELETE: http://localhost:portnumber/api/friendships/cancel/{requestId}
        [SwaggerOperation(Summary = "Cancel friend request", Description = "Hủy yêu cầu kết bạn.")]
        [HttpDelete("cancel/{requestId:Guid}")]
        public async Task<IActionResult> CancelFriendRequest(Guid requestId)
        {
            var friendshipDomainModel = await friendshipRepository.CancelRequestAsync(requestId);
            if (friendshipDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<FriendshipDto>(friendshipDomainModel));
        }

        //Check friendship status
        // GET: http://localhost:portnumber/api/friendships/status/{userId1}/{userId2}
        // Cần sửa lại truyền vào usedId của mình, và userId của người bạn
        [SwaggerOperation(Summary = "Check friendship status", Description = "Kiểm tra trạng thái kết bạn.")]
        [HttpGet("status/{userId1:Guid}/{userId2:Guid}")]
        public async Task<IActionResult> CheckFriendshipStatus(Guid userId, Guid friendId)
        {
            var status = await friendshipRepository.CheckFriendshipStatusAsync(userId, friendId);
            return Ok(status);
        }

        //Get friends list
        // GET: http://localhost:portnumber/api/friendships/friends/{userId}
        [SwaggerOperation(Summary = "Get friends list", Description = "Lấy danh sách bạn bè.")]
        [HttpGet("friends/{userId:Guid}")]
        public async Task<IActionResult> GetFriendsList(Guid userId)
        {
            var friends = await friendshipRepository.ListFriendsAsync(userId);
            return Ok(mapper.Map<List<UserDto>>(friends));
        }

        //Get friend requests
        // GET: http://localhost:portnumber/api/friendships/requests/{userId}  
        [SwaggerOperation(Summary = "Get friend requests", Description = "Lấy danh sách yêu cầu kết bạn.")]
        [HttpGet("requests/{userId:Guid}")]
        public async Task<IActionResult> GetFriendRequests(Guid userId)
        {
            var requests = await friendshipRepository.ListFriendRequestsAsync(userId);
            return Ok(mapper.Map<List<FriendshipDto>>(requests));
        }
        //Reject friend request
        // DELETE: http://localhost:portnumber/api/friendships/reject/{requestId}
        [SwaggerOperation(Summary = "Reject friend request", Description = "Từ chối yêu cầu kết bạn.")]
        [HttpDelete("reject/{requestId:Guid}")]
        public async Task<IActionResult> RejectFriendRequest(Guid requestId)
        {
            var friendshipDomainModel = await friendshipRepository.RejectRequestAsync(requestId);
            if (friendshipDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<FriendshipDto>(friendshipDomainModel));
        }
        //Remove friend
        // DELETE: http://localhost:portnumber/api/friendships/remove/{friendId}
        [SwaggerOperation(Summary = "Remove friend", Description = "Xóa bạn bè.")]
        [HttpDelete("remove/{friendId:Guid}")]
        public async Task<IActionResult> RemoveFriend(Guid friendId)
        {
            var friendshipDomainModel = await friendshipRepository.RemoveFriendAsync(friendId);
            if (friendshipDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<FriendshipDto>(friendshipDomainModel));
        }
    }
}
