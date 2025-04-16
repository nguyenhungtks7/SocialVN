using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

// cần sửa lại giá trị trả về DTO
namespace SocialVN.API.Controllers
{
    // https:localhost:portnumber/api/users
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly IImageRepository imageRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UsersController(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IImageRepository imageRepository)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.imageRepository = imageRepository;
        }

       
        [SwaggerOperation(Summary = "Update Profile ", Description = "Cập nhật hồ sơ.")]
        [HttpPut("me")]
        public async Task<IActionResult> Update([FromForm] UpdateUserProfileDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new ApiResponse<string>(400, "Không xác định được người dùng.", null));
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest(new ApiResponse<string>(400, "Không tìm thấy người dùng", null));

            user.FullName = dto.FullName;
            user.BirthDate = dto.BirthDate;
            user.Occupation = dto.Occupation;
            user.Location = dto.Location;

            if (dto.Avatar != null)
            {
                //var uploadsFolder = Path.Combine(env.ContentRootPath, "Images");
                //Directory.CreateDirectory(uploadsFolder);

                //var ext = Path.GetExtension(dto.Avatar.FileName);
                //var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Avatar.FileName)}";
                //var filePath = Path.Combine(uploadsFolder, fileName);

                //using var stream = new FileStream(filePath, FileMode.Create);
                //await dto.Avatar.CopyToAsync(stream);
                //var request = httpContextAccessor.HttpContext.Request;
                //var urlFilePath = $"{request.Scheme}://{request.Host}{request.PathBase}/Images/{fileName}";
                //user.AvatarPath = "/Images/{fileName}";
                if (!imageRepository.IsValidImage(dto.Avatar))
                {
                    return BadRequest(new ApiResponse<string>(400, "Tệp không hợp lệ. Chỉ chấp nhận các tệp .jpg, .jpeg, .png và kích thước không quá 10MB.", null));
                }
                var avatarPath = await imageRepository.UploadAsync(dto.Avatar);
                user.AvatarPath = avatarPath;
            }
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<string>(400, "Cập nhật thất bại",null));
            }
            return Ok(new ApiResponse<ApplicationUser>(200, "Cập nhật thành công", null));
        }

        [SwaggerOperation(Summary = "Get all users", Description = "Lấy danh sách tất cả người dùng với tùy chọn lọc, sắp xếp và phân trang.")]
        [HttpGet("Search")]
        public async Task<IActionResult> Search(
        [FromQuery] string? filterOn,
        [FromQuery] string? filterQuery,
        [FromQuery] string? sortBy,
                      [FromQuery] bool? isAscending,
                      [FromQuery] int pageNumber = 1,
                       [FromQuery] int pageSize = 10)
        {
            var query = userManager.Users.AsQueryable();

            // Lọc (filter)
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                switch (filterOn.ToLower())
                {
                    case "fullname":
                        query = query.Where(x => x.FullName != null && x.FullName.Contains(filterQuery));
                        break;
                    case "location":
                        query = query.Where(x => x.Location != null && x.Location.Contains(filterQuery));
                        break;
                    case "occupation":
                        query = query.Where(x => x.Occupation != null && x.Occupation.Contains(filterQuery));
                        break;
                }
            }

            // Sắp xếp (sort)
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "fullname":
                        query = (isAscending ?? true) ? query.OrderBy(x => x.FullName) : query.OrderByDescending(x => x.FullName);
                        break;
                    case "birthdate":
                        query = (isAscending ?? true) ? query.OrderBy(x => x.BirthDate) : query.OrderByDescending(x => x.BirthDate);
                        break;
                    case "created":
                        query = (isAscending ?? true) ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id); // tạm thời dùng Id để sort
                        break;
                }
            }

            // Tổng số bản ghi
            var totalRecords = await query.CountAsync();

            // Phân trang (paging)
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //var userDtos = mapper.Map<List<UserDto>>(users);

            return Ok(new ApiResponse<object>(
                200,
                null,
                new
                {
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = users
                }));
        }


        [HttpGet]
        public async Task<IActionResult> GetById()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy người dùng", null));
            }

            return Ok(new ApiResponse<ApplicationUser>(200, null, user));
        }


        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy người dùng", null));
            }

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<string>(400, "Xoá thất bại", string.Join(", ", result.Errors.Select(e => e.Description))));
            }

            return Ok(new ApiResponse<string>(200, "Xoá thành công", null));
        }


        //private readonly IUserRepository userRepository;
        //private readonly IMapper mapper;

        //public UsersController(IUserRepository userRepository, IMapper mapper)
        //{
        //    this.userRepository = userRepository;
        //    this.mapper = mapper;
        //}

        ////GET all users
        //// GET: http:localhost:portnumber/api/users
        //[HttpGet]
        //public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
        //    [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
        //    [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        //{
        //    var userDomainModel = await userRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);
        //    return Ok(mapper.Map<List<UserDto>>(userDomainModel));
        //}

        ////GET user By Id
        //// GET: http:localhost:portnumber/api/users/{id}
        //[HttpGet("{id:Guid}")]
        //public async Task<IActionResult> GetById(Guid id)
        //{
        //    var userDomainModel = await userRepository.GetByIdAsync(id);
        //    if (userDomainModel == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(mapper.Map<UserDto>(userDomainModel));
        //}

        //// Create user
        //// POST: http:localhost:portnumber/api/users
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] AddUserRequestDto addUserRequestDto)
        //{
        //    var userDomainModel = mapper.Map<User>(addUserRequestDto);
        //    await userRepository.CreateAsync(userDomainModel);
        //    return Ok(mapper.Map<UserDto>(userDomainModel));
        //}

        ////Update user
        //// PUT: http:localhost:portnumber/api/users/{id}
        //[HttpPut("{id:Guid}")]
        //public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserRequestDto updateUserRequestDto)
        //{
        //    var userDomainModel = await userRepository.GetByIdAsync(id);
        //    if (userDomainModel == null)
        //    {
        //        return NotFound();
        //    }
        //    mapper.Map(updateUserRequestDto, userDomainModel);
        //    await userRepository.UpdateAsync(userDomainModel);
        //    return Ok(mapper.Map<UserDto>(userDomainModel));
        //}

        ////Delete user
        //// DELETE: http:localhost:portnumber/api/users/{id}
        //[HttpDelete("{id:Guid}")]
        //public async Task<IActionResult> Delete([FromRoute] Guid id)
        //{
        //    var userDomainModel = await userRepository.DeleteAsync(id);
        //    if (userDomainModel == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(mapper.Map<UserDto>(userDomainModel));
        //}
    }
}
