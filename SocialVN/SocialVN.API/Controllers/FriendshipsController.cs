using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialVN.API.Enums;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FriendshipsController : ControllerBase
{
    private readonly IFriendshipRepository friendshipRepository;
    private readonly IMapper mapper;

    public FriendshipsController(
        IFriendshipRepository friendshipRepository,
        IMapper mapper)
    {
        this.friendshipRepository = friendshipRepository;
        this.mapper = mapper;
    }

    // 1. Send friend request
    [SwaggerOperation(Summary = "Send friend request", Description = "Gửi yêu cầu kết bạn.")]
    [HttpPost]
    public async Task<IActionResult> SendFriendRequest([FromBody] AddFriendshipRequestDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ApiResponse<string>(401, "Bạn cần đăng nhập để gửi yêu cầu", null));

        bool exists = await friendshipRepository.IsFriendRequestExistsAsync(userId, dto.ReceiverId.ToString());
        if (exists)
            return BadRequest(new ApiResponse<string>(400, "Yêu cầu kết bạn đã tồn tại", null));

        var entity = new Friendship
        {
            RequesterId = userId,
            ReceiverId  = dto.ReceiverId.ToString(),
            StatusEnum  = FriendshipStatus.Pending,
            CreatedAt   = DateTime.UtcNow,
            UpdatedAt   = DateTime.UtcNow
        };

        await friendshipRepository.SendRequestAsync(entity);
 
        return Ok(new ApiResponse<FriendshipDto>(200, "Gửi yêu cầu thành công", null));
    }

    // 2. Accept friend request
    [SwaggerOperation(Summary = "Accept friend request", Description = "Chấp nhận yêu cầu kết bạn.")]
    [HttpPut("accept/{requestId:Guid}")]
    public async Task<IActionResult> AcceptFriendRequest(Guid requestId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ApiResponse<string>(401, "Bạn cần đăng nhập để chấp nhận yêu cầu", null));

        var entity = await friendshipRepository.GetByIdAsync(requestId);
        if (entity == null)
            return NotFound(new ApiResponse<string>(404, "Yêu cầu không tồn tại", null));

        if (entity.ReceiverId != userId)
            return StatusCode(403, new ApiResponse<string>(403, "Bạn không có quyền chấp nhận yêu cầu này", null));

        entity = await friendshipRepository.AcceptRequestAsync(requestId);
       
        return Ok(new ApiResponse<FriendshipDto>(200, "Chấp nhận kết bạn thành công", null));
    }

    // 3. Cancel friend request
    [SwaggerOperation(Summary = "Cancel friend request", Description = "Hủy yêu cầu kết bạn.")]
    [HttpDelete("cancel/{requestId:Guid}")]
    public async Task<IActionResult> CancelFriendRequest(Guid requestId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ApiResponse<string>(401, "Bạn cần đăng nhập để hủy yêu cầu", null));

        var entity = await friendshipRepository.GetByIdAsync(requestId);
        if (entity == null)
            return NotFound(new ApiResponse<string>(404, "Yêu cầu không tồn tại", null));

        if (entity.RequesterId != userId)
            return StatusCode(403, new ApiResponse<string>(403, "Bạn không có quyền hủy yêu cầu này", null));

        entity = await friendshipRepository.CancelRequestAsync(requestId);
       
        return Ok(new ApiResponse<FriendshipDto>(200, "Hủy yêu cầu thành công", null));
    }

    // 4. Reject friend request
    [SwaggerOperation(Summary = "Reject friend request", Description = "Từ chối yêu cầu kết bạn.")]
    [HttpDelete("reject/{requestId:Guid}")]
    public async Task<IActionResult> RejectFriendRequest(Guid requestId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ApiResponse<string>(401, "Bạn cần đăng nhập để từ chối yêu cầu", null));

        var entity = await friendshipRepository.GetByIdAsync(requestId);
        if (entity == null)
            return NotFound(new ApiResponse<string>(404, "Yêu cầu không tồn tại", null));

        if (entity.ReceiverId != userId)
            return StatusCode(403, new ApiResponse<string>(403, "Bạn không có quyền từ chối yêu cầu này", null));

        entity = await friendshipRepository.RejectRequestAsync(requestId);
    
        return Ok(new ApiResponse<FriendshipDto>(200, "Từ chối yêu cầu thành công", null));
    }

    // 5. Remove friend
    [SwaggerOperation(Summary = "Remove friend", Description = "Xóa bạn bè.")]
    [HttpDelete("remove/{friendshipId:Guid}")]
    public async Task<IActionResult> RemoveFriend(Guid friendshipId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ApiResponse<string>(401, "Bạn cần đăng nhập để xóa bạn", null));

        var entity = await friendshipRepository.GetByIdAsync(friendshipId);
        if (entity == null)
            return NotFound(new ApiResponse<string>(404, "Quan hệ bạn bè không tồn tại", null));

        if (entity.RequesterId != userId && entity.ReceiverId != userId)
            return StatusCode(403, new ApiResponse<string>(403, "Bạn không có quyền xóa quan hệ này", null));

        entity = await friendshipRepository.RemoveFriendAsync(friendshipId);
       
        return Ok(new ApiResponse<FriendshipDto>(200, "Xóa bạn thành công", null));
    }

  
    [SwaggerOperation(Summary = "Check friendship status", Description = "Kiểm tra trạng thái kết bạn.")]
    [HttpGet("status/{otherUserId:Guid}")]
    public async Task<IActionResult> CheckFriendshipStatus(Guid otherUserId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ApiResponse<string>(401, "Bạn cần đăng nhập để kiểm tra trạng thái", null));

        var status = await friendshipRepository
            .CheckFriendshipStatusAsync(userId, otherUserId.ToString());

        return Ok(new ApiResponse<FriendshipStatus>(200, null, status));
    }

    // 7. Get friends list
    [SwaggerOperation(Summary = "Get friends list", Description = "Lấy danh sách bạn bè.")]
    [HttpGet("friends")]
    public async Task<IActionResult> GetFriendsList()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ApiResponse<string>(401, "Bạn cần đăng nhập để xem danh sách bạn bè", null));

        var friends = await friendshipRepository.ListFriendsAsync(Guid.Parse(userId));
        var resultDtos = mapper.Map<List<UserDto>>(friends);
        return Ok(new ApiResponse<List<UserDto>>(200, null, resultDtos));
    }

    // 8. Get incoming friend requests
    [SwaggerOperation(Summary = "Get friend requests", Description = "Lấy danh sách yêu cầu kết bạn đến.")]
    [HttpGet("requests")]
    public async Task<IActionResult> GetFriendRequests()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ApiResponse<string>(401, "Bạn cần đăng nhập để xem yêu cầu kết bạn", null));

        var requests = await friendshipRepository.ListFriendRequestsAsync(Guid.Parse(userId));
        var resultDtos = mapper.Map<List<FriendshipDto>>(requests);
        return Ok(new ApiResponse<List<FriendshipDto>>(200, null, resultDtos));
    }
}
