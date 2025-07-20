using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs;
using System.Threading.Tasks;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserRegisterDto request);
    Task<TokenResponseDto?> LoginAsync(UserDto request);
    Task<bool> ConfirmEmailAsync(Guid userId, string token);
    Task<TokenResponseDto?> RefreshTokensAsync(string refreshToken);
    Task<bool> UpdateUserRoleAsync(UpdateRoleDto request);
    Task<User> GetUserFromTokenAsync(string token);
    bool IsAdult(string? dobString);

    // --- New methods for Forgot Password feature ---
    Task<bool> SendPasswordResetEmailAsync(string email);
    Task<bool> ResetPasswordAsync(Guid userId, string token, string newPassword);
    // --- End new methods ---
}