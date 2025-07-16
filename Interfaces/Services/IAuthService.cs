using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserRegisterDto request);
    Task<TokenResponseDto?> LoginAsync(UserDto request);
    Task<TokenResponseDto?> RefreshTokensAsync(string refreshToken);
    Task<bool> UpdateUserRoleAsync(UpdateRoleDto request);
    Task<User> GetUserFromTokenAsync(string token);
    bool IsAdult(string? dobString); 
}