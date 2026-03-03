using Microsoft.AspNetCore.Mvc;
using Core.Services;
using Core.DTOs.ResponseDto;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserResponseDto>>> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> UpdateProfile(Guid id, [FromBody] Core.DTOs.User.UserProfileDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.UpdateProfileAsync(id, dto);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] Core.DTOs.User.ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _userService.ChangePasswordAsync(id, dto);
            if (!success)
                return BadRequest(new { message = "Invalid current password or user not found" });

            return Ok(new { message = "Password changed successfully" });
        }
    }
}
