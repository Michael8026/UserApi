using api.Data;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static api.Helpers.CommunicationData;

namespace api.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthService(
            UserManager<AppUser> userManager,
            ITokenService tokenService,
            IUserProfileRepository userProfileRepository,
            SignInManager<AppUser> signInManager, IEmailService emailService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _userProfileRepository = userProfileRepository;
            _signInManager = signInManager;
            _emailService = emailService;
            _context = context;
        }

        public async Task<IEnumerable<UserProfile>> GetAllUsersAsync()
        {
            var users = await _userProfileRepository.GetAllAsync();

            return users;
        }


        public async Task<NewUserDto> RegisterAsync(RegisterDto registerDto)
        {
            var appUser = new AppUser
            {
                UserName = registerDto.Email.ToLowerInvariant(),
                Email = registerDto.Email.ToLowerInvariant()
            };

            var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

            if (!createdUser.Succeeded)
            {
                throw new Exception("User not created");
            }

            var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

            if (!roleResult.Succeeded)
            {
                throw new Exception("Role not added");
            }

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

            //Generate confirmation token
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);


            var emailUser = new EmailUserDTO
            {
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
            };

            //await _emailService.SendConfirmationEmail(emailUser, confirmationToken);

            //var token = _tokenService.CreateToken(appUser);

            var newUser = new NewUserDto
            {
                Email = appUser.Email,
                //Token = "Check your email for a confirmation link "
                Token = confirmationToken
            };


            return newUser;
        }

        public async Task<NewUserDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email.ToLowerInvariant());

            if (user == null)
                return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return null;

            if (!user.EmailConfirmed)
                throw new UnauthorizedAccessException("Please confirm your email before logging in.");

            return new NewUserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            };
        }

        public async Task<string> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new Exception("User not found");

            }
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception("User not deleted");
            }

            return "User deleted successfully";

        }


        public async Task<string> UpdateUserAsync(string id, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found!");

            var userProfile = await _userProfileRepository.GetByIdAsync(id);
            if (userProfile == null)
                throw new Exception("User profile not found!");

            user.Email = updateUserDto.Email.ToLowerInvariant();
            user.UserName = updateUserDto.Email.ToLowerInvariant();

            var updatedUser = await _userManager.UpdateAsync(user);

            if (!updatedUser.Succeeded)
            {
                throw new Exception("User not updated");
            }

            await _userProfileRepository.UpdateAsync(id, updateUserDto);

            return "User profile updated successfully";
        }

        public async Task<UserProfile> GetUserByEmailAsync(string email)
        {
            var user = await _userProfileRepository.GetByEmailAsync(email.ToLowerInvariant());

            return user;
        }

        public async Task<string> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return "User not found";

            if (user.EmailConfirmed)
                return "Email is already confirmed.";

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return "Email confirmed successfully.";

            return "Email confirmation failed.";
        }

        public async Task<AccessToken> GenerateAndStoreAccessTokenAsync(string email, DateTime expiry)
        {
            if (expiry <= DateTime.UtcNow)
                throw new ArgumentException("Expiry must be in the future.");

            if ((expiry - DateTime.UtcNow).TotalDays > 3)
                throw new ArgumentException("Expiry cannot be more than 3 days.");

            var token = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

            var accessToken = new AccessToken
            {
                Token = token,
                Email = email,
                Expiry = expiry
            };

            _context.AccessTokens.Add(accessToken);
            await _context.SaveChangesAsync();

            return accessToken;
        }

        public async Task<string> VerifyAccessTokenAsync(string token, string userEmail)
        {
            if (string.IsNullOrWhiteSpace(token) || token.Length != 6)
                return "Invalid token format";

            var accessToken = await _context.AccessTokens
                .FirstOrDefaultAsync(t => t.Token == token && t.Email.ToLower() == userEmail.ToLower());

            if (accessToken == null)
                return "Token not found or not assigned to this user";

            if (accessToken.Expiry <= DateTime.UtcNow)
                return "Token has expired";

            return "Token is valid";
        }





    }
}
