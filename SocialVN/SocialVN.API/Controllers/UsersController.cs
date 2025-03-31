using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;

namespace SocialVN.API.Controllers
{
    // https:localhost:portnumber/api/users
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        //GET all users
        // GET: http:localhost:portnumber/api/users
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var userDomainModel = await userRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);
            return Ok(mapper.Map<List<UserDto>>(userDomainModel));
        }

        //GET user By Id
        // GET: http:localhost:portnumber/api/users/{id}
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userDomainModel = await userRepository.GetByIdAsync(id);
            if (userDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<UserDto>(userDomainModel));
        }

        // Create user
        // POST: http:localhost:portnumber/api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddUserRequestDto addUserRequestDto)
        {
            var userDomainModel = mapper.Map<User>(addUserRequestDto);
            await userRepository.CreateAsync(userDomainModel);
            return Ok(mapper.Map<UserDto>(userDomainModel));
        }

        //Update user
        // PUT: http:localhost:portnumber/api/users/{id}
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserRequestDto updateUserRequestDto)
        {
            var userDomainModel = await userRepository.GetByIdAsync(id);
            if (userDomainModel == null)
            {
                return NotFound();
            }
            mapper.Map(updateUserRequestDto, userDomainModel);
            await userRepository.UpdateAsync(userDomainModel);
            return Ok(mapper.Map<UserDto>(userDomainModel));
        }

        //Delete user
        // DELETE: http:localhost:portnumber/api/users/{id}
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var userDomainModel = await userRepository.DeleteAsync(id);
            if (userDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<UserDto>(userDomainModel));
        }
    }
}
