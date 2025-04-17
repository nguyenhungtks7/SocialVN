using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace SocialVN.API.Controllers
{
    // https:localhost:portnumber/api/posts
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository postRepository;
        private readonly IMapper mapper;
        private readonly IImageRepository imageRepository;

        public PostsController(IPostRepository postRepository, IMapper mapper, IImageRepository imageRepository)
        {
            this.postRepository = postRepository;
            this.mapper = mapper;
            this.imageRepository = imageRepository;
        }
        //  GET all posts
        // GET: http:localhost:portnumber/api/posts

        [SwaggerOperation(Summary = "Get all posts", Description = "Lấy danh sách tất cả các bài đăng.")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await postRepository.GetAllAsync());
        }
        // GET post By Id
        // GET: http:localhost:portnumber/api/posts/{id}
        [SwaggerOperation(Summary = "Get post by id", Description = "Lấy bài đăng theo id.")]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var postDomainModel = await postRepository.GetByIdAsync(id);

            if (postDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<PostDto>(postDomainModel));
        }

        // Create post
        // POST: http:localhost:portnumber/api/posts
        [SwaggerOperation(Summary = "Create post", Description = "Tạo bài đăng mới.")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddPostRequestDto post)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse<string>(401, "Không xác định được người dùng", null));
            }
            var postDomainModel = mapper.Map<Post>(post);
            if (postDomainModel.ImageFile != null)
            {
                if (!imageRepository.IsValidImage(postDomainModel.ImageFile))
                {
                    return BadRequest(new ApiResponse<string>(400, "Tệp không hợp lệ. Chỉ chấp nhận các tệp .jpg, .jpeg, .png và kích thước không quá 10MB.", null));
                }
                var imgPath = await imageRepository.UploadToImgurAsync(postDomainModel.ImageFile);
                postDomainModel.Image = imgPath;
            }
            postDomainModel.UserId = userId;
            postDomainModel.UpdatedAt = DateTime.Now;
            postDomainModel.CreatedAt =DateTime.Now;
            await postRepository.CreateAsync(postDomainModel);
            return Ok(mapper.Map<PostDto>(postDomainModel));
        }

        // Update post by id
        // PUT: http:localhost:portnumber/api/posts/{id}
        [SwaggerOperation(Summary = "Update post", Description = "Cập nhật bài đăng theo id.")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePostRequestDto post)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse<string>(401, "Không xác định được người dùng", null));
            }

            var existingPost = await postRepository.GetByIdAsync(id);

            if (existingPost == null)
            {
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy bài viết", null));
            }

            if (existingPost.UserId != userId)
            {
                return Forbid(new ApiResponse<string>(403, "Bạn không có quyền chỉnh sửa bài viết này", null).ToString());
            }
            var postDomainModel = mapper.Map<Post>(post);
            postDomainModel.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (postDomainModel.ImageFile != null)
            {
                if (!imageRepository.IsValidImage(postDomainModel.ImageFile))
                {
                    return BadRequest(new ApiResponse<string>(400, "Tệp không hợp lệ. Chỉ chấp nhận các tệp .jpg, .jpeg, .png và kích thước không quá 10MB.", null));
                }
                var imgPath = await imageRepository.UploadToImgurAsync(postDomainModel.ImageFile);
                postDomainModel.Image = imgPath;
            }
            postDomainModel.UpdatedAt = DateTime.Now;
            
            var updatedPost = await postRepository.UpdateAsync(id, postDomainModel);
            if (updatedPost == null)
            {
                return NotFound();
            }
            return Ok(new ApiResponse<PostDto>(200, "Cập nhật bài viết thành công", mapper.Map<PostDto>(updatedPost)));
        }
        // Delete post by id
        // DELETE: http:localhost:portnumber/api/posts/{id}
        [SwaggerOperation(Summary = "Delete post", Description = "Xóa bài đăng theo id.")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse<string>(401, "Không xác định được người dùng", null));
            }

            var existingPost = await postRepository.GetByIdAsync(id);

            if (existingPost == null)
            {
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy bài viết", null));
            }

            if (existingPost.UserId != userId)
            {
                return StatusCode(403, new ApiResponse<string>(403, "Bạn không có quyền xóa bài viết này", null));
            }

            var postDomainModel = await postRepository.DeleteAsync(id);
            if (postDomainModel == null)
            {
                return NotFound();
            }
            return Ok(new ApiResponse<PostDto>(200, "Xóa bài viết thành công", mapper.Map<PostDto>(postDomainModel)));
        }

    }
}
