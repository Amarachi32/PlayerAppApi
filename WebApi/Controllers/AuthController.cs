using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.DTO;
using WebApi.Interface;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route(("User"))]
        [SwaggerOperation(Summary = "register a user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto register)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.RegisterUserAsync(register);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                var authResponse = new AuthResponse
                {
                    Message = "User successfully created",
                };

                return CreatedAtAction(nameof(RegisterUser), authResponse);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route(("Auth"))]
        [SwaggerOperation(Summary = "Authenticate and login in a user")]
        public async Task<AuthResponse> Authenticate([FromBody] LoginDto loginDto)
        {
            AuthResponse response = await _authService.UserLogin(loginDto);
            return response;
        }

    }
}
