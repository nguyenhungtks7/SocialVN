using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;
using Swashbuckle.AspNetCore.Annotations;

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

        // Create like
        // POST: http:localhost:portnumber/api/likes

        [SwaggerOperation(Summary = "Create a new like", Description = "Tạo lượt thích mới cho một bài đăng cụ thể của người dùng.")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddLikeRequestDto like)
        {
            var likeDomainModel = mapper.Map<Like>(like);
            await likeRepository.CreateLikeAsync(likeDomainModel);
            return Ok(mapper.Map<LikeDto>(likeDomainModel));
        }
        //Delete like by id
        // DELETE: http:localhost:portnumber/api/likes/{id}
        [SwaggerOperation(Summary = "Delete a like", Description = "Xóa lượt thích theo id.")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var likeDomainModel = await likeRepository.DeleteLikeAsync(id);
            if (likeDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<LikeDto>(likeDomainModel));
        }

        // Chekk like exist
        // GET: http:localhost:portnumber/api/likes/{postId}/{userId}
        [SwaggerOperation(Summary = "Check if a like exists", Description = "Kiểm tra xem lượt thích có tồn tại hay không.")]
        [HttpGet("{postId:Guid}/{userId:Guid}")]
        public async Task<IActionResult> IsLikeExist(Guid postId, Guid userId)
        {
            var isLikeExist = await likeRepository.IsLikeExistAsync(postId, userId);
            return Ok(isLikeExist);
        }
        // Count likes
        // GET: http:localhost:portnumber/api/likes/{postId}
        [SwaggerOperation(Summary = "Count likes", Description = "Đếm số lượt thích cho một bài đăng cụ thể.")]
        [HttpGet("{postId:Guid}")]
        public async Task<IActionResult> CountLikes(Guid postId)
        {
            var count = await likeRepository.CountLikesAsync(postId);
            return Ok(count);
        }
        // Get users who liked a post
        // GET: http:localhost:portnumber/api/likes/{postId}
        [SwaggerOperation(Summary = "Get users who liked a post", Description = "Lấy danh sách người dùng đã thích một bài đăng cụ thể.")]
        [HttpGet("{postId:Guid}/users")]
        public async Task<IActionResult> GetUsersWhoLiked(Guid postId)
        {
            var users = await likeRepository.GetUsersWhoLikedAsync(postId);
            return Ok(mapper.Map<List<UserDto>>(users));
        }

    }
}
