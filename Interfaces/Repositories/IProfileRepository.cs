using AskHire_Backend.Models.Entities;

namespace AskHire_Backend.Interfaces.Repositories
{
    public interface IProfileRepository
    {
        Task<User?> GetUserByIdAsync(Guid Id);
        Task<bool> SaveChangesAsync();
    }
}