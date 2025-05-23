using api.Dtos.Account;
using api.Models;

namespace api.Interfaces
{
    public interface IAuthService
    {
        Task<string> DeleteUserAsync(string id);
        Task<IEnumerable<UserProfile>> GetAllUsersAsync();
        Task<UserProfile> GetUserByEmailAsync(string email);
        Task<NewUserDto> LoginAsync(LoginDto loginDto);
        Task<NewUserDto> RegisterAsync(RegisterDto registerDto);
        Task<string> UpdateUserAsync(string id, UpdateUserDto updateUserDto);
    }
}
