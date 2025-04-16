using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using System.Web;

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
        
        [SwaggerOperation(Summary = "Register", Description = "Đăng ký.")]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var existingUser = await userManager.FindByNameAsync(registerRequestDto.Username);
            if (existingUser != null)
            {
                return BadRequest(new ApiResponse<string>(400, "Người dùng đã tồn tại", null));
            }
            

            var identityUser = new ApplicationUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username,
                UpdatedAt =DateTime.Now,
                CreatedAt = DateTime.Now
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
                        return Ok(new ApiResponse<string>(200, "Người dùng đã đăng ký thành công", null));
                    }

                //}
            }
            return BadRequest(new ApiResponse<string>(400, "Đã xảy ra sự cố", null));

        }
        //POST: /api/Auth/Login
        [SwaggerOperation(Summary = "Login", Description = "Đăng nhập.")]
        [HttpPost]
        [Route("Login")]
        
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);
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

        [SwaggerOperation(Summary = "Login", Description = "Đăng nhập vào hệ thống bằng email và mật khẩu. Nếu thành công, OTP sẽ được gửi để xác minh.")]
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

        [SwaggerOperation(Summary = "ForgotPassword", Description = "Quên mật khẩu.")]
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new ApiResponse<string>(400, "Người dùng không tồn tại", null));
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            // Gửi email chứa token đến người dùng
            var encodedTokKen = HttpUtility.UrlEncode(token);

            //Ví dụ đường dẫn
            var callbackUrl = $"{Request.Scheme}://{Request.Host}/api/Auth/ResetPassword?email={model.Email}&token={encodedTokKen}";
         
     
            return Ok(new ApiResponse<string>(200, "Vui lòng kiểm tra email của bạn để đặt lại mật khẩu", callbackUrl));
        }

        [SwaggerOperation(Summary = "ResetPassword", Description = "Đặt lại mật khẩu.")]
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new ApiResponse<string>(400, "Người dùng không tồn tại", null));
            }
            var decodedToken = HttpUtility.UrlDecode(model.Token);
            var result = await userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new ApiResponse<string>(200, "Đặt lại mật khẩu thành công", null));
            }
            return BadRequest(new ApiResponse<string>(400, "Đặt lại mật khẩu không thành công", null));
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
