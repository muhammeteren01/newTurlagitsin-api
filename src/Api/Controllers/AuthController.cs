using Microsoft.AspNetCore.Mvc;
using Core.Services;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if user exists
            if (await _userService.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new { message = "Email already registered" });

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone = request.Phone,
                CreatedAt = DateTime.UtcNow
            };

            await _userService.AddAsync(user);

            var token = GenerateJwtToken(user);

            var result = new
            {
                token = token,
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    phone = user.Phone,
                    avatar = user.Avatar
                }
            };

            return Ok(result);
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.Where(u => u.Email == request.Email)
                .FirstOrDefaultAsync();

            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials" });

            var token = GenerateJwtToken(user);

            var result = new
            {
                token = token,
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    phone = user.Phone,
                    avatar = user.Avatar,
                    location = user.Location
                }
            };

            return Ok(result);
        }

        // POST: api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _userService.Where(u => u.Email == request.Email)
                .FirstOrDefaultAsync();

            if (user == null)
                return Ok(new { message = "If the email exists, a reset link will be sent" });

            // TODO: Send password reset email
            // Generate reset token and save it, then send email

            return Ok(new { message = "If the email exists, a reset link will be sent" });
        }

        // POST: api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            // TODO: Verify reset token
            // For now, just validate new password

            if (string.IsNullOrEmpty(request.NewPassword) || request.NewPassword.Length < 6)
                return BadRequest(new { message = "Password must be at least 6 characters" });

            // Update password in database
            // var user = await _userService.GetByIdAsync(userId);
            // user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            // await _userService.UpdateAsync(user);

            return Ok(new { message = "Password reset successful" });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? "");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class RegisterRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Phone { get; set; }
    }

    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public required string Email { get; set; }
    }

    public class ResetPasswordRequest
    {
        public required string Token { get; set; }
        public required string NewPassword { get; set; }
    }
}
