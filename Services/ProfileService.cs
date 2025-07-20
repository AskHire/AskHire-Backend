using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Interfaces.Services;
using AskHire_Backend.Models.Entities;

namespace AskHire_Backend.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _repo;

        public ProfileService(IProfileRepository repo) => _repo = repo;

        public async Task<User?> GetProfileAsync(Guid userId) =>
            await _repo.GetUserByIdAsync(userId);

        public async Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var user = await _repo.GetUserByIdAsync(userId);
            if (user == null) return false;

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Gender = dto.Gender ?? user.Gender;
            user.DOB = dto.DOB ?? user.DOB;
            user.MobileNumber = dto.MobileNumber ?? user.MobileNumber;
            user.Address = dto.Address ?? user.Address;

            return await _repo.SaveChangesAsync();
        }

        public async Task<bool> SetAvatarAsync(Guid userId, string avatarFileName)
        {
            var allowedAvatars = new[] { "avatar1.png", "avatar2.png", "avatar3.png", "avatar4.png" };
            if (!allowedAvatars.Contains(avatarFileName)) return false;

            var user = await _repo.GetUserByIdAsync(userId);
            if (user == null) return false;

            user.ProfilePictureUrl = $"/avatars/{avatarFileName}";
            return await _repo.SaveChangesAsync();
        }
    }

}