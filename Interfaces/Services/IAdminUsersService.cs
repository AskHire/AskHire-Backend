using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services.Interfaces
{
    public interface IAdminUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(Guid id, User user);
        Task<bool> UpdateUserRoleAsync(Guid id, string newRole);
        Task<bool> DeleteUserAsync(Guid id);

        Task<IEnumerable<User>> GetAdminsAsync();
        Task<IEnumerable<User>> GetCandidatesAsync();
        Task<IEnumerable<User>> GetManagersAsync();

        Task<bool> DeleteAdminAsync(Guid id);
        Task<bool> DeleteCandidateAsync(Guid id);
        Task<bool> DeleteManagerAsync(Guid id);

        Task<int> GetTotalUsersAsync();
    }
}
