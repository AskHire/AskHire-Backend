using AskHire_Backend.Models.Entities;

public interface IAuthRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid userId);
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken); // New method
    Task<bool> AnyAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
    Task UpdateAsync(User user);
}