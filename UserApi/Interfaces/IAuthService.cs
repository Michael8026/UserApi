using api.Dtos.Account;
using api.Models;

namespace api.Interfaces
{
    public interface IAuthService
    {
        Task<string> ConfirmEmailAsync(string userId, string token);
        Task<string> DeleteUserAsync(string id);
        Task<AccessToken> GenerateAndStoreAccessTokenAsync(string email, DateTime expiry);
        Task<IEnumerable<UserProfile>> GetAllUsersAsync();
        Task<UserProfile> GetUserByEmailAsync(string email);
        Task<NewUserDto> LoginAsync(LoginDto loginDto);
        Task<NewUserDto> RegisterAsync(RegisterDto registerDto);
        Task<string> UpdateUserAsync(string id, UpdateUserDto updateUserDto);
        Task<string> VerifyAccessTokenAsync(string token, string userEmail);
    }
}
