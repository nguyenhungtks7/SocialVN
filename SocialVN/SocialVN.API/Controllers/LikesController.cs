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
        private readonly IPostRepository postRepository;
        private readonly IMapper mapper;
        public LikesController(ILikeRepository likeRepository, IMapper mapper, IPostRepository postRepository)
        {
            this.likeRepository = likeRepository;
            this.mapper = mapper;
            this.postRepository=postRepository;
        }
        [SwaggerOperation(Summary = "Toggle like", Description = "Like nếu chưa like, hoặc Unlike nếu đã like.")]
        [Authorize]
        [HttpPost("Toggle-like")]
        public async Task<IActionResult> ToggleLike([FromBody] AddLikeRequestDto likeDto)
        {
          var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized(new ApiResponse<string>(401, "Không xác định được người dùng", null));
            }
            var post = await postRepository.GetByIdAsync(likeDto.PostId);
            if (post == null)
            {
                return NotFound(new ApiResponse<string>(404, "Bài viết không tồn tại", null));
            }

            var existingLike = await likeRepository.GetLikeByUserAndPostAsync(userId, likeDto.PostId);

            if (existingLike != null)
            {

                var deletedLike = await likeRepository.DeleteLikeAsync(existingLike.Id);
                return Ok(new ApiResponse<string>(200, "Đã bỏ thích bài viết", null));
            }
            else
            {
              
                var likeDomainModel = mapper.Map<Like>(likeDto);
                likeDomainModel.UserId = userId;
                likeDomainModel.CreatedAt = DateTime.UtcNow;
                likeDomainModel.UpdatedAt   = DateTime.UtcNow;
                var createdLike = await likeRepository.CreateLikeAsync(likeDomainModel);
                return Ok(new ApiResponse<LikeDto>(201, "Đã thích bài viết thành công", mapper.Map<LikeDto>(createdLike)));
            }

        }

      
    

    }
}
