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
    
    public class UsersController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
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
        [Authorize]

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
                return BadRequest(new ApiResponse<string>(401, "Không xác định được người dùng.", null));
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest(new ApiResponse<string>(404, "Không tìm thấy người dùng", null));

            user.FullName = dto.FullName;
            user.BirthDate = dto.BirthDate;
            user.Occupation = dto.Occupation;
            user.Location = dto.Location;
            user.LivingPlace = dto.LivingPlace;
            user.Gender = dto.Gender.Value.GetDisplayName();
            user.PhoneNumber = dto.PhoneNumber;

            if (dto.Avatar != null)
            {
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
                return StatusCode(500, new ApiResponse<string>(500, "Cập nhật thất bại do lỗi hệ thống.", null));
            }
            return Ok(new ApiResponse<ApplicationUser>(200, "Cập nhật thành công", null));
        }
        [Authorize(Roles = "Admin")]
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

        [Authorize]
        [SwaggerOperation(Summary = "Get profile", Description = "Hiện thị thông tin  người dùng")]
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
        [Authorize]
        [SwaggerOperation(Summary = "Delete user", Description = "Xóa người dùng")]
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse<string>(401, "Không xác định được người dùng.", null));

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy người dùng", null));
            }

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return StatusCode(500, new ApiResponse<string>(500, "Xoá thất bại do lỗi hệ thống", string.Join(", ", result.Errors.Select(e => e.Description))));
            }
            await signInManager.SignOutAsync();
            return Ok(new ApiResponse<string>(200, "Xoá thành công", null));
        }


    }
}
