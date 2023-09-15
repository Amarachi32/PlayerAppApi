using Microsoft.AspNetCore.Identity;
using WebApi.DTO;

namespace WebApi.Interface
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterDto register);
        Task<AuthResponse> UserLogin(LoginDto loginDto);
    }
}
