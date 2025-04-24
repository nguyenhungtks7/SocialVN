using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.Security.Claims;

namespace SocialVN.API.Controllers
{
    // https:localhost:portnumber/api/likes
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    
    public class LikesController : ControllerBase
    {
        private readonly ILikeRepository likeRepository;
        private readonly IMapper mapper;
        public LikesController(ILikeRepository likeRepository, IMapper mapper)
        {
            this.likeRepository = likeRepository;
            this.mapper = mapper;
        }
        [SwaggerOperation(Summary = "Toggle like", Description = "Like nếu chưa like, hoặc Unlike nếu đã like.")]
        [Authorize]
        [HttpPost("{postId:Guid}")]
        public async Task<IActionResult> ToggleLike([FromBody] AddLikeRequestDto like)
        {
          var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new ApiResponse<string>(401, "Không xác định được người dùng", null));
            }
            var existingLike = await likeRepository.GetLikeByUserAndPostAsync(userId, like.PostId);

            if (existingLike != null)
            {

                var deletedLike = await likeRepository.DeleteLikeAsync(existingLike.Id);
                return Ok(new ApiResponse<string>(200, "Đã bỏ thích bài viết", null));
            }
            else
            {
              
                var likeDomainModel = mapper.Map<Like>(like);
                likeDomainModel.UserId = userId;

                var createdLike = await likeRepository.CreateLikeAsync(likeDomainModel);
                return Ok(new ApiResponse<LikeDto>(201, "Đã thích bài viết thành công", mapper.Map<LikeDto>(createdLike)));
            }

        }
        //// Create like
        //// POST: http:localhost:portnumber/api/likes

        //[SwaggerOperation(Summary = "Like a post", Description = "Tạo lượt thích mới cho một bài đăng cụ thể.")]
        //[HttpPost]
        //public async Task<IActionResult> LikePost([FromBody] AddLikeRequestDto like)
        //{
        //    // Lấy UserId từ người dùng hiện tại
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Hoặc tùy theo cách bạn lưu trữ thông tin người dùng

        //    var likeDomainModel = mapper.Map<Like>(like);
        //    likeDomainModel.UserId = userId; // Gán UserId từ người dùng hiện tại
        //    likeDomainModel.Usid = userId;  // Gán Usid là chính UserId của người dùng hiện tại

        //    var result = await likeRepository.CreateLikeAsync(likeDomainModel);


        //        return Ok(new ApiResponse<LikeDto>(201, "Đã thích bài viết thành công", mapper.Map<LikeDto>(likeDomainModel)));

        //}


        //// Delete like by id
        //// DELETE: http://localhost:portnumber/api/likes/{id}
        //[SwaggerOperation(Summary = "Delete a like", Description = "Xóa lượt thích theo id.")]
        //[HttpDelete("{id:Guid}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var likeDomainModel = await likeRepository.DeleteLikeAsync(id);
        //    if (likeDomainModel == null)
        //    {
        //        return NotFound(new ApiResponse<string>(404, "Lượt thích không tồn tại", null));
        //    }
        //    return Ok(new ApiResponse<LikeDto>(200, "Xóa lượt thích thành công", mapper.Map<LikeDto>(likeDomainModel)));
        //}

        // Check like exist
        // GET: http://localhost:portnumber/api/likes/{postId}/{userId}
        [SwaggerOperation(Summary = "Check if a like exists", Description = "Kiểm tra xem lượt thích có tồn tại hay không.")]
        [HttpGet("{postId:Guid}/{userId:Guid}")]
        public async Task<IActionResult> IsLikeExist(Guid postId, Guid userId)
        {
            var isLikeExist = await likeRepository.IsLikeExistAsync(postId, userId);
            return Ok(new ApiResponse<bool>(200, "Kiểm tra lượt thích thành công", isLikeExist));
        }

        // Count likes
        // GET: http://localhost:portnumber/api/likes/{postId}
        [SwaggerOperation(Summary = "Count likes", Description = "Đếm số lượt thích cho một bài đăng cụ thể.")]
        [HttpGet("{postId:Guid}")]
        public async Task<IActionResult> CountLikes(Guid postId)
        {
            var count = await likeRepository.CountLikesAsync(postId);
            return Ok(new ApiResponse<int>(200, "Đếm lượt thích thành công", count));
        }
        // Get users who liked a post
        // GET: http:localhost:portnumber/api/likes/{postId}
        //[SwaggerOperation(Summary = "Get users who liked a post", Description = "Lấy danh sách người dùng đã thích một bài đăng cụ thể.")]
        //[HttpGet("{postId:Guid}/users")]
        //public async Task<IActionResult> GetUsersWhoLiked(Guid postId)
        //{
        //    var users = await likeRepository.GetUsersWhoLikedAsync(postId);
        //    return Ok(mapper.Map<List<UserDto>>(users));
        //}

    }
}
