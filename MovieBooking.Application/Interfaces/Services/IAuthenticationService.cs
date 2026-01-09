using MovieBooking.Application.DTOs.Authentication;

namespace MovieBooking.Application.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponseDto> RegisterAsync(RegisterRequestDto Dto);
        Task<AuthenticationResponseDto> LoginAsync(LoginRequestDto Dto);
    }

}
