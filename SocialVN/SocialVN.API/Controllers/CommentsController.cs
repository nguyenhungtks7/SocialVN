using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialVN.API.Controllers
{
    // https:localhost:portnumber/api/comments
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
        [SwaggerOperation(Summary = "Get all comment", Description = "Truy xuất tất cả các bình luận với tùy chọn sắp xếp và phân trang.")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? sortBy, [FromQuery] bool? isAscending,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var commentsDomainModel = await commentsRepository.GetAllCommentsAsync(sortBy, isAscending ?? true, pageNumber, pageSize);
            return Ok(mapper.Map<List<CommentDto>>(commentsDomainModel));
        }
        // GET comment By Id
        // GET: http:localhost:portnumber/api/comments/{id}
        [SwaggerOperation(Summary = "Get comment by id", Description = "Truy xuất bình luận theo id.")]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var commentDomainModel = await commentsRepository.GetCommentByIdAsync(id);
            if (commentDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<CommentDto>(commentDomainModel));
        }
        // Create comment
        // POST: http:localhost:portnumber/api/comments
        [SwaggerOperation(Summary = "Create comment", Description = "Tạo bình luận mới.")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddCommentRequestDto comment)
        {
            var commentDomainModel = mapper.Map<Comment>(comment);
            await commentsRepository.CreateCommentAsync(commentDomainModel);
            return Ok(mapper.Map<CommentDto>(commentDomainModel));
        }
        // Update comment by id
        // PUT: http:localhost:portnumber/api/comments/{id}
        [SwaggerOperation(Summary = "Update comment", Description = "Cập nhật bình luận theo id.")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommentRequestDto comment)
        {
            var commentDomainModel = mapper.Map<Comment>(comment);
            var updatedComment = await commentsRepository.UpdateCommentAsync(id, commentDomainModel);
            if (updatedComment == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<CommentDto>(updatedComment));
        }
        // Delete comment by id
        //  DELETE: http:localhost:portnumber/api/comments/{id}
        [SwaggerOperation(Summary = "Delete comment", Description = "Xóa bình luận theo id.")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedComment = await commentsRepository.DeleteCommentAsync(id);
            if (deletedComment == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<CommentDto>(deletedComment));
        }

    }
}
