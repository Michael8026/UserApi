using api.Dtos.Account;
using api.Models;

namespace api.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<List<UserProfile>> GetAllAsync();
        Task<UserProfile> CreateAsync(UserProfile userProfile);
        Task<UserProfile> GetByEmailAsync(string email);
        Task<UserProfile> GetByIdAsync(string id);
        Task<UserProfile> UpdateAsync(string id, UpdateUserDto updateUserDto);


    }
}
