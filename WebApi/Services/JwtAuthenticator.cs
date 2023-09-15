using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Entities;
using WebApi.Interface;

namespace WebApi.Services
{
    public class JwtAuthenticator : IJWTAuthenticator
    {
        private readonly IConfiguration _configuration;

        public JwtAuthenticator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<JwtToken> GenerateJwtToken(AppUser user)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new();
            string jwtConfig = _configuration.GetSection("JwtConfig:Secret").Value;

            //var key = Encoding.ASCII.GetBytes(jwtConfig);
            var key = Encoding.UTF8.GetBytes(jwtConfig);

            IdentityOptions _options = new();
            var claims = new List<Claim>
            {

                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(_options.ClaimsIdentity.UserIdClaimType, user.Id),
                new Claim(_options.ClaimsIdentity.UserNameClaimType, user.UserName),
                new Claim(_options.ClaimsIdentity.RoleClaimType, user.Id)
            };


            string issuer = _configuration.GetSection("JwtConfig:Issuer").Value;
            string audience = _configuration.GetSection("JwtConfig:Audience").Value;
            string expire = _configuration.GetSection("JwtConfig:Expires").Value;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return new JwtToken
            {
                Token = jwtToken,
                Issued = DateTime.Now,
                Expires = tokenDescriptor.Expires
            };
        }
    }
}
