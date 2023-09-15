using WebApi.Entities;

namespace WebApi.Interface
{
    public interface IJWTAuthenticator
    {
        Task<JwtToken> GenerateJwtToken(AppUser user);
    }

}
