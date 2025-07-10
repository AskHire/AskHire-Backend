using AskHire_Backend.Models.Entities;

namespace AskHire_Backend.Interfaces.Services
{
    public interface IProfileService
    {
        Task<User?> GetProfileAsync(Guid userId);
        Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
        Task<bool> SetAvatarAsync(Guid userId, string avatarFileName);
    }

}
