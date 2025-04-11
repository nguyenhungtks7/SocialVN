using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialVN.API.Controllers
{
    // https:localhost:portnumber/api/posts
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository postRepository;
        private readonly IMapper mapper;
        
        public PostsController(IPostRepository postRepository, IMapper mapper)
        {
            this.postRepository = postRepository;
            this.mapper = mapper;
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
            var postDomainModel = mapper.Map<Post>(post);
            await postRepository.CreateAsync(postDomainModel);
            return Ok(mapper.Map<PostDto>(postDomainModel));
        }

        // Update post by id
        // PUT: http:localhost:portnumber/api/posts/{id}
        [SwaggerOperation(Summary = "Update post", Description = "Cập nhật bài đăng theo id.")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePostRequestDto post)
        {
            var postDomainModel = mapper.Map<Post>(post);
            var updatedPost = await postRepository.UpdateAsync(id, postDomainModel);
            if (updatedPost == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<PostDto>(updatedPost));
        }
        // Delete post by id
        // DELETE: http:localhost:portnumber/api/posts/{id}
        [SwaggerOperation(Summary = "Delete post", Description = "Xóa bài đăng theo id.")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var postDomainModel = await postRepository.DeleteAsync(id);
            if (postDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<PostDto>(postDomainModel));
        }

    }
}
