using PiggyBank.Application.DTOs.Auth;

namespace PiggyBank.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<string> GenerateJwtToken(string userId, string email);
    }
}
