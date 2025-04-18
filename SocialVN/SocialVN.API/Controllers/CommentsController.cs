using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SocialVN.API.Controllers
{
    // https:localhost:portnumber/api/comments
    [Authorize(Roles = "Admin,User")]
    [Route("api/[controller]")]
    [ApiController]

    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository commentsRepository;
        private readonly Mapper mapper;

        public CommentsController(ICommentRepository commentsRepository ,Mapper mapper)
        {
            this.commentsRepository = commentsRepository;
            this.mapper = mapper;
        }
        // GET all comments
        // GET: http:localhost:portnumber/api/comments

        // GET all comments with sorting & pagination
        [SwaggerOperation(Summary = "Retrieve a list of comments")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var comments  = await commentsRepository.GetAllCommentsAsync(sortBy, isAscending ?? true, pageNumber, pageSize);
            var dto = mapper.Map<List<CommentDto>>(comments);
            return Ok(new ApiResponse<List<CommentDto>>(200, "Lấy danh sách bình luận thành công", dto));
        }

        // GET by id
  
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var comment = await commentsRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy bình luận", null));
            }

            return Ok(new ApiResponse<CommentDto>(200, "Lấy bình luận thành công", mapper.Map<CommentDto>(comment)));
        }

        // CREATE
        [SwaggerOperation(Summary = "Create comment")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddCommentRequestDto commentDto)
        {
           
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized(new ApiResponse<string>(401, "Không thể xác định người dùng", null));
            }

            var comment = mapper.Map<Comment>(commentDto);
            comment.UserId = userId.ToString();
            comment.CreatedAt = DateTime.UtcNow;
            comment.UpdatedAt = DateTime.UtcNow;

            var created = await commentsRepository.CreateCommentAsync(comment);

            return Ok(new ApiResponse<string>(201, "Người dùng đã tạo bình luận thành công", null));
        }

        // UPDATE
        [SwaggerOperation(Summary = "Update comment")]
        [Authorize]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommentRequestDto commentDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized(new ApiResponse<string>(401, "Không thể xác định người dùng", null));
            }

            var commentDomain = mapper.Map<Comment>(commentDto);
            commentDomain.UserId = userId.ToString(); // Đảm bảo userId không bị thay đổi

            var updated = await commentsRepository.UpdateCommentAsync(id, commentDomain);
            if (updated == null)
            {
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy bình luận cần cập nhật", null));
            }

            return Ok(new ApiResponse<string>(200, "Cập nhật bình luận thành công", null));
        }

        // DELETE
        [SwaggerOperation(Summary = "Deletecomment")]
        [Authorize]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await commentsRepository.DeleteCommentAsync(id);
            if (deleted == null)
            {
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy bình luận cần xóa", null));
            }

            return Ok(new ApiResponse<string>(200, "Xóa bình luận thành công", null));
        }

    }
}
