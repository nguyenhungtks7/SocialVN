using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;
using System.Security.Claims;

namespace SocialVN.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;

        }
        //POST: /api/Auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var existingUser = await userManager.FindByNameAsync(registerRequestDto.Username);
            if (existingUser != null)
            {
                return BadRequest(new ApiResponse<string>(400, "Người dùng đã tồn tại! Vui lòng đăng nhập.", null));
            }
            

            var identityUser = new ApplicationUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };
            var indentityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);
            if (indentityResult.Succeeded)
            {
                //// Add roles to this User
                //if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                //{

                    indentityResult = await userManager.AddToRolesAsync(identityUser, new List<string> { "User" });
                    if (indentityResult.Succeeded)
                    {
                        return Ok(new ApiResponse<string>(200, "Người dùng đã đăng ký! Vui lòng đăng nhập", null));
                    }

                //}
            }
            return BadRequest(new ApiResponse<string>(400, "Đã xảy ra sự cố", null));

        }
        //POST: /api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Usename);
            if (user != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (checkPasswordResult)
                {
                    // Tạo OTP giả lập (ngẫu nhiên 6 chữ số)
                    var otp = new Random().Next(100000, 999999).ToString();

                    // Lưu tạm OTP này vào database hoặc memory để xác thực sau
                    // Ở đây demo lưu vào User Claims ( có thể tuỳ chọn cache/memory nếu muốn)
                    await userManager.RemoveClaimsAsync(user, await userManager.GetClaimsAsync(user));
                    await userManager.AddClaimAsync(user, new Claim("OTP", otp));
                    await userManager.AddClaimAsync(user, new Claim("OTP_ValidUntil", DateTime.UtcNow.AddMinutes(5).ToString("o")));

                    return Ok(new ApiResponse<string>(200, "OTP được gửi thành công. Vui lòng xác minh.", otp));
                }
            }
            return BadRequest(new ApiResponse<string>(400, "Tên người dùng hoặc mật khẩu không chính xác", null));
        }
        [HttpPost]
        [Route("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null) return BadRequest(new ApiResponse<string>(400, "Người dùng không tồn tại", null));

            var claims = await userManager.GetClaimsAsync(user);
            var otpClaim = claims.FirstOrDefault(c => c.Type == "OTP");
            var validUntilClaim = claims.FirstOrDefault(c => c.Type == "OTP_ValidUntil");

            if (otpClaim == null || validUntilClaim == null)
                return BadRequest(new ApiResponse<string>(400, "OTP không tồn tại hoặc đã hết hạn", null));

            if (otpClaim.Value != dto.OTP)
                return BadRequest(new ApiResponse<string>(400, "OTP không chính xác", null));

            if (DateTime.TryParse(validUntilClaim.Value, out var validUntil))
            {
                if (DateTime.UtcNow > validUntil)
                    return BadRequest(new ApiResponse<string>(400, "OTP đã hết hạn", null));
            }

            // Xoá claim OTP sau khi xác minh thành công
            await userManager.RemoveClaimAsync(user, otpClaim);
            await userManager.RemoveClaimAsync(user, validUntilClaim);

            var roles = await userManager.GetRolesAsync(user);
            var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList());

            return Ok(new ApiResponse<string>(200, "Đăng nhập thành công", jwtToken)); ;
        }
        //[HttpPost]
        //[Route("Login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        //{

        //    var user = await userManager.FindByEmailAsync(loginRequestDto.Usename);
        //    if (user != null)
        //    {
        //        var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
        //        if (checkPasswordResult)
        //        {
        //            // Get Roles for this user
        //            var roles = await userManager.GetRolesAsync(user);
        //            if (roles != null)
        //            {
        //                // Create Token
        //                var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList());
        //                var response = new LoginResponseDto
        //                {
        //                    JwtToken = jwtToken,

        //                };
        //                return Ok(response);
        //            }
        //        }
        //    }
        //    return BadRequest("Tên người dùng hoặc mật khẩu không chính xác");
        //}
    }
}
