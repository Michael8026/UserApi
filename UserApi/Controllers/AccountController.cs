using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IUserProfileRepository _userProfileRepository;
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager, IUserProfileRepository userProfileRepository)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userProfileRepository = userProfileRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userProfileRepository.GetAllAsync();

            return Ok(users);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var appUser = new AppUser
                {
                    UserName = registerDto.Email.ToLowerInvariant(),
                    Email = registerDto.Email.ToLowerInvariant(),
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

                    if (roleResult.Succeeded)
                    {
                        var userProfile = new UserProfile
                        {
                            Id = appUser.Id,
                            Email = registerDto.Email,
                            FirstName = registerDto.FirstName,
                            LastName = registerDto.LastName,
                            Gender = registerDto.Gender,
                            Age = registerDto.Age
                        };

                        await _userProfileRepository.CreateAsync(userProfile);

                        return Ok(new NewUserDto
                        {
                            Email = appUser.Email,
                            Token = _tokenService.CreateToken(appUser)
                        });
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception e)
            {

                return StatusCode(500, e);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email.ToLower());

            if (user == null)
                return Unauthorized("Invalid Email!");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return Unauthorized("Email not found or password incorrect");

            return Ok(new NewUserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            });

        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound("User not found!");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "User deleted successfully" });
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found!");

            var userProfile = await _userProfileRepository.GetByIdAsync(id);
            if (userProfile == null)
                return NotFound("User profile not found!");

            user.Email = updateUserDto.Email.ToLower();
            user.UserName = updateUserDto.Email.ToLower();

            var updatedUser = await _userManager.UpdateAsync(user);
            if (!updatedUser.Succeeded)
                return BadRequest(updatedUser.Errors);

            await _userProfileRepository.UpdateAsync(id, updateUserDto);

            return Ok("Userprofile updated successfully");

        }


        [HttpGet("by_email")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userProfileRepository.GetByEmailAsync(email);

            return Ok(user);
        }







    }
}
