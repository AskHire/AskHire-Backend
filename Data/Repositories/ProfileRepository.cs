using AskHire_Backend.Data.Entities;
using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Models.Entities;

namespace AskHire_Backend.Data.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _context;
        public ProfileRepository(AppDbContext context) => _context = context;

        public async Task<User?> GetUserByIdAsync(Guid id) =>
            await _context.Users.FindAsync(id);

        public async Task<bool> SaveChangesAsync() =>
            await _context.SaveChangesAsync() > 0;
    }

}