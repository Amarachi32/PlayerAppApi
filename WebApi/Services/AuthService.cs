using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using WebApi.DTO;
using WebApi.Entities;
using WebApi.Interface;

namespace WebApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IJWTAuthenticator _jWTAuthenticator;
        private AppUser? _user;
        public AuthService(IMapper mapper, UserManager<AppUser> userManager, IJWTAuthenticator jWTAuthenticator)
        {
            _mapper = mapper;
            _userManager = userManager;
            _jWTAuthenticator = jWTAuthenticator;

        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterDto register)
        {
            _user = await _userManager.FindByEmailAsync(register.Email);
            if (_user != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User already exist" });
            }
            var newUser = _mapper.Map<AppUser>(register);
            var result = await _userManager.CreateAsync(newUser, register.Password);
            if (result.Succeeded)
            {
                return result;
            }
            return result;
        }

        public async Task<AuthResponse> UserLogin(LoginDto loginDto)
        {
            _user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (_user == null)
                return new AuthResponse { Message = "A user with this email address does not exist." };
            var result = (_user != null && await _userManager.CheckPasswordAsync(_user, loginDto.Password));

            if (result)
            {
                JwtToken token = await _jWTAuthenticator.GenerateJwtToken(_user);
                var authResponse = new AuthResponse()
                {
                    Token = token.Token.ToString(),
                    Message = "Login Successful"
                };
                return authResponse;

            }
            return new AuthResponse()
            {
                Message = "Incorrect Username and password"
            };


        }
    }
}
