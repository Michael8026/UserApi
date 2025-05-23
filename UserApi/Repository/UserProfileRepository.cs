using api.Data;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{

    public class UserProfileRepository : IUserProfileRepository
    {

        private readonly ApplicationDbContext _context;
        public UserProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile> CreateAsync(UserProfile userProfile)
        {
            await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();
            return userProfile;

        }


        public async Task<List<UserProfile>> GetAllAsync()
        {
            return await _context.UserProfiles.ToListAsync();
        }

        public async Task<UserProfile> GetByEmailAsync(string email)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserProfile> GetByIdAsync(string id)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UserProfile> UpdateAsync(string id, UpdateUserDto updateUserDto)
        {
            var existingUser = await _context.UserProfiles.FirstOrDefaultAsync(x => x.Id == id);

            if (existingUser == null)
            {
                return null;
            }

            existingUser.Email = updateUserDto.Email;
            existingUser.FirstName = updateUserDto.FirstName;
            existingUser.LastName = updateUserDto.LastName;
            existingUser.Gender = updateUserDto.Gender;
            existingUser.Age = updateUserDto.Age;

            await _context.SaveChangesAsync();

            return existingUser;
        }
    }
}
