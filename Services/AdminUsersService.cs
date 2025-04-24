using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Repositories.Interfaces;
using AskHire_Backend.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUserRepository _userRepo;

        public AdminUserService(IAdminUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userRepo.GetAllAsync();

        public async Task<User> GetUserByIdAsync(Guid id) => await _userRepo.GetByIdAsync(id);

        public async Task<User> CreateUserAsync(User user) => await _userRepo.AddAsync(user);

        public async Task<bool> UpdateUserAsync(Guid id, User user)
        {
            var existing = await _userRepo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.FirstName = user.FirstName;
            existing.LastName = user.LastName;
            existing.Email = user.Email;
            existing.Gender = user.Gender;
            existing.DOB = user.DOB;
            existing.NIC = user.NIC;
            existing.MobileNumber = user.MobileNumber;
            existing.Address = user.Address;
            existing.Role = user.Role;
            existing.RefreshToken = user.RefreshToken;
            existing.RefreshTokenExpiryTime = user.RefreshTokenExpiryTime;

            return await _userRepo.UpdateAsync(existing);
        }

        public async Task<bool> UpdateUserRoleAsync(Guid id, string newRole)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return false;

            user.Role = newRole;
            return await _userRepo.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(Guid id) => await _userRepo.DeleteAsync(id);

        public async Task<IEnumerable<User>> GetAdminsAsync() => await _userRepo.GetByRoleAsync("Admin");
        public async Task<IEnumerable<User>> GetCandidatesAsync() => await _userRepo.GetByRoleAsync("Candidate");
        public async Task<IEnumerable<User>> GetManagersAsync() => await _userRepo.GetByRoleAsync("Manager");

        public async Task<bool> DeleteAdminAsync(Guid id) => await DeleteUserWithRoleValidation(id, "Admin");
        public async Task<bool> DeleteCandidateAsync(Guid id) => await DeleteUserWithRoleValidation(id, "Candidate");
        public async Task<bool> DeleteManagerAsync(Guid id) => await DeleteUserWithRoleValidation(id, "Manager");

        private async Task<bool> DeleteUserWithRoleValidation(Guid id, string role)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null || user.Role != role) return false;
            return await _userRepo.DeleteAsync(id);
        }
    }
}
