using api.Dtos.Account;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserProfileRepository _userProfileRepository;
        public AccountController(IUserProfileRepository userProfileRepository, IAuthService authService)
        {
            _userProfileRepository = userProfileRepository;
            _authService = authService;
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _authService.GetAllUsersAsync();

            return Ok(users);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);

            return Ok(result);


        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
                return Unauthorized("Email not found or password incorrect");

            return Ok(result);
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var message = await _authService.DeleteUserAsync(id);
                return Ok(new { Message = message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var message = await _authService.UpdateUserAsync(id, updateUserDto);

                return Ok(new { Message = message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }

        }


        [HttpGet("by_email")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _authService.GetUserByEmailAsync(email);

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }


        [HttpGet("confirm_email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            var resultMessage = await _authService.ConfirmEmailAsync(email, token);

            if (resultMessage == "Email confirmed successfully.")
                return Ok(resultMessage);

            return BadRequest(resultMessage);
        }


        [Authorize]
        [HttpPost("generate_access_token")]
        public async Task<IActionResult> GenerateToken([FromQuery] string email, [FromQuery] DateTime expiry)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var token = await _authService.GenerateAndStoreAccessTokenAsync(email, expiry);
                return Ok(token);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while generating the token.");
            }
        }

        [Authorize]
        [HttpPost("verify_access_token")]
        public async Task<IActionResult> VerifyToken([FromQuery] string token, [FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
                return BadRequest("Token and email are required.");

            var result = await _authService.VerifyAccessTokenAsync(token, email);

            if (result != "Token is valid")
                return BadRequest(result);

            return Ok(result);
        }



    }
}
