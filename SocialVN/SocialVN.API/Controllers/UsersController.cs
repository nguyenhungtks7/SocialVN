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

            //Map DTO Domain Model
            var userDomainModel = await userRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);
            return Ok(mapper.Map<List<UserDto>>(userDomainModel));

        }
        //GET user by id
        // GET: http:localhost:portnumber/api/users




        // Create user
        // POST: http:localhost:portnumber/apo/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddUserRequestDto addUserRequestDto)
        {
            var useDomainModel = mapper.Map<User>(addUserRequestDto);
            await userRepository.CreteAsync(useDomainModel);
            return Ok(mapper.Map<UserDto>(useDomainModel));
        }
    }
}
