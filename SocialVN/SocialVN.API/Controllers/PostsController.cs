using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SocialVN.API.Enums;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace SocialVN.API.Controllers
{
    // https:localhost:portnumber/api/posts
 
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository postRepository;
        private readonly IFriendshipRepository friendshipRepository;
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
        [Authorize]
        [HttpGet("timeline")]
        [SwaggerOperation(Summary = "Get timeline", Description = "Lấy bài đăng gần nhất của bạn bè theo trang, mặc định 10 bản ghi.")]
        public async Task<IActionResult> GetTimeline([FromQuery] int pageSize = 10)
        {
            int pageNumber = 1;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse<string>(401, "Bạn cần đăng nhập để xem timeline", null));

            var posts = await postRepository.GetTimelineAsync(userId, pageNumber, pageSize);
            var dtoList = mapper.Map<List<PostDto>>(posts);
            return Ok(new ApiResponse<object>(
           200,
           null,
           new
           {
               PageNumber = pageNumber,
               PageSize = pageSize,
               Data = dtoList
           }));
        }
        // GET post By Id
        // GET: http:localhost:portnumber/api/posts/{id}
        [Authorize]
        [SwaggerOperation(Summary = "Get post by id", Description = "Lấy bài đăng theo id.")]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var postDomainModel = await postRepository.GetByIdAsync(id);
            if (postDomainModel == null)
            {
                return NotFound();
            }
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isOwner = postDomainModel.UserId == currentUserId;
            if (!isOwner)
            {
                switch (postDomainModel.Status)
                {
                    case PostStatus.Private:
                        return StatusCode(403, new ApiResponse<string>(403, "Bài viết đang ở chế độ riêng tư", null)); // Không cho xem
                    case PostStatus.FriendsOnly:
                        var friendshipStatus = await friendshipRepository.CheckFriendshipStatusAsync(currentUserId, postDomainModel.UserId);
                        if (friendshipStatus != FriendshipStatus.Accepted)
                            return StatusCode(403, new ApiResponse<string>(403, "Chỉ bạn bè mới được xem bài viết này", null)); ;
                        break;
                    case PostStatus.Public:
                        break; // OK
                }
            }
            var postDto = new PostDto
            {
                UserId = postDomainModel.UserId,
                Content = postDomainModel.Content,
                Image = postDomainModel.Image,
                Status = postDomainModel.Status,
                User = postDomainModel.User == null ? null : new UserDto
                {
                    Id = postDomainModel.UserId,
                    UserName = postDomainModel.User.UserName,
                    Email = postDomainModel.User?.Email,
                    FullName = postDomainModel.User?.FullName,
                    BirthDate = postDomainModel.User?.BirthDate,
                    Occupation = postDomainModel.User?.Occupation,
                    Location = postDomainModel.User?.Location,
                    LivingPlace = postDomainModel.User?.LivingPlace,
                    Gender = postDomainModel.User?.Gender,
                    PhoneNumber = postDomainModel.User?.PhoneNumber,
                    AvatarPath = postDomainModel.User?.AvatarPath,
                    CreatedAt = postDomainModel.User?.CreatedAt ?? default,
                    UpdatedAt = postDomainModel.User?.UpdatedAt ?? default
                }
            };

            //// Trả về bài viết trong ApiResponse
            return Ok(new ApiResponse<PostDto>(200, null, postDto));
            //var postDto = mapper.Map<PostDto>(postDomainModel);

            //return Ok(new ApiResponse<PostDto>(200, null, postDto));
        }
            


        // Create post
        // POST: http:localhost:portnumber/api/posts
        [Authorize]
        [SwaggerOperation(Summary = "Create post", Description = "Tạo bài đăng mới.")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] AddPostRequestDto post)
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
            return Ok(new ApiResponse<string>(201, "Người dùng đã tạo bài đăng thành công", null));
        }

        // Update post by id
        // PUT: http:localhost:portnumber/api/posts/{id}
        [Authorize]
        [SwaggerOperation(Summary = "Update post", Description = "Cập nhật bài đăng theo id.")]
        [HttpPut("{id:Guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdatePostRequestDto post)
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
                return BadRequest(new ApiResponse<string>(403, "Bạn không có quyền chỉnh sửa bài viết này", null));
            }
            var postDomainModel = mapper.Map<Post>(post);
            postDomainModel.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (existingPost.ImageFile != null)
            {
                if (!imageRepository.IsValidImage(existingPost.ImageFile))
                {
                    return BadRequest(new ApiResponse<string>(400, "Tệp không hợp lệ. Chỉ chấp nhận các tệp .jpg, .jpeg, .png và kích thước không quá 10MB.", null));
                }
                var imgPath = await imageRepository.UploadToImgurAsync(existingPost.ImageFile);
                postDomainModel.Image = imgPath;
            }
            postDomainModel.Content = existingPost.Content;
            postDomainModel.Status = existingPost.Status;
            postDomainModel.UpdatedAt = DateTime.Now;
            
            var updatedPost = await postRepository.UpdateAsync(id, postDomainModel);
            if (updatedPost == null)
            {
                return NotFound();
            }
            return Ok(new ApiResponse<PostDto>(200, "Cập nhật bài viết thành công", null));
        }
        // Delete post by id
        // DELETE: http:localhost:portnumber/api/posts/{id}
        [Authorize]
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
