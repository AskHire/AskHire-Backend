using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;

namespace AskHire_Backend.Repositories.Interfaces
{
    public interface IAdminUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(Guid id);
        Task<User> AddAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(Guid id);

        Task<int> GetTotalUsersAsync();

        Task<IEnumerable<User>> GetByRoleAsync(string role);
    }
}
