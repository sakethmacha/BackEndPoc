using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.DTOs.Authentication;
using MovieBooking.Application.Interfaces.Services;

namespace MovieBooking.Api.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService AuthenticationService;

        public AuthenticationController(IAuthenticationService AuthenticationService)
        {
            this.AuthenticationService = AuthenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            return Ok(await AuthenticationService.RegisterAsync(registerRequestDto));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            return Ok(await AuthenticationService.LoginAsync(loginRequestDto));
        }
    }

}
