using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using SocialVN.API.Enums;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

// cần sửa lại giá trị trả về DTO
namespace SocialVN.API.Controllers
{
    // https:localhost:portnumber/api/users
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UsersController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly IImageRepository imageRepository;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UsersController(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IImageRepository imageRepository, IMapper mapper)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.imageRepository = imageRepository;
            this.mapper = mapper;
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
            user.LivingPlace = dto.LivingPlace;
            if (dto.Gender != null)
            {
                user.Gender = dto.Gender.Value.GetDisplayName(); 
            }
            user.PhoneNumber = dto.PhoneNumber;

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
                var avatarPath = await imageRepository.UploadToImgurAsync(dto.Avatar);
                user.AvatarPath = avatarPath;
            }
            user.UpdatedAt = DateTime.Now;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<string>(400, "Cập nhật thất bại", null));
            }
            return Ok(new ApiResponse<ApplicationUser>(200, "Cập nhật thành công", null));
        }

        [SwaggerOperation(Summary = "Search", Description = "Lấy danh sách tất cả người dùng với tùy chọn lọc, sắp xếp và phân trang.")]
        [HttpGet("Search")]
        public async Task<IActionResult> Search(
        [FromQuery] UserFilterField? filterOn,
        [FromQuery] string? filterQuery,
        [FromQuery] UserFilterField? sortBy,
                      [FromQuery] bool? isAscending,
                      [FromQuery] int pageNumber = 1,
                       [FromQuery] int pageSize = 10)
        {
            var query = userManager.Users.AsQueryable();

            // Lọc (filter)
            if (!string.IsNullOrWhiteSpace(filterQuery) && filterOn.HasValue)
            {
                switch (filterOn)
                {
                    case UserFilterField.FullName:
                        query = query.Where(x => x.FullName != null && x.FullName.Contains(filterQuery));
                        break;
                    case UserFilterField.Location:
                        query = query.Where(x => x.Location != null && x.Location.Contains(filterQuery));
                        break;
                    case UserFilterField.Occupation:
                        query = query.Where(x => x.Occupation != null && x.Occupation.Contains(filterQuery));
                        break;
                }
            }

            // Sắp xếp (sort)
            if (sortBy.HasValue)
            {
                switch (sortBy)
                {
                    case UserFilterField.FullName:
                        query = isAscending ?? true ? query.OrderBy(x => x.FullName) : query.OrderByDescending(x => x.FullName);
                        break;
                    case UserFilterField.BirthDate:
                        query = isAscending ?? true ? query.OrderBy(x => x.BirthDate) : query.OrderByDescending(x => x.BirthDate);
                        break;
                    case UserFilterField.CreatedAt:
                        query = isAscending ?? true ? query.OrderBy(x => x.CreatedAt) : query.OrderByDescending(x => x.CreatedAt);
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
                    Data = mapper.Map<List<UserDto>>(users)
                }));
        }


        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new ApiResponse<string>(400, "Không xác định được người dùng.", null));
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy người dùng", null));
            }


            return Ok(new ApiResponse<UserDto>(200, null, mapper.Map<UserDto>(user)));
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
