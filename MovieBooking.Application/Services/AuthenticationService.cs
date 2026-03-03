using Microsoft.Extensions.Configuration;
using MovieBooking.Application.DTOs.Authentication;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Constants;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace MovieBooking.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository UserRepository;
        private readonly IConfiguration Configuration;

        public AuthenticationService(
            IUserRepository userRepository,
            IConfiguration configuration)
        {
            UserRepository = userRepository;
            Configuration = configuration;
        }

        public async Task<AuthenticationResponseDto> RegisterAsync(
            RegisterRequestDto registerRequestDto)
        {
            var existingUser =
                await UserRepository.GetByEmailAsync(registerRequestDto.Email);

            if (existingUser != null)
                throw new InvalidOperationException(
                    MessageStrings.EmailAlreadyRegistered);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Name = registerRequestDto.Name,
                Email = registerRequestDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(
                    registerRequestDto.Password),
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await UserRepository.AddAsync(user);

            return GenerateJwt(user);
        }

        public async Task<AuthenticationResponseDto> LoginAsync(
            LoginRequestDto loginRequestDto)
        {
            var user =
                await UserRepository.GetByEmailAsync(loginRequestDto.Email)
                ?? throw new InvalidOperationException(
                    MessageStrings.InvalidCredentials);

            if (!BCrypt.Net.BCrypt.Verify(
                    loginRequestDto.Password,
                    user.Password))
                throw new InvalidOperationException(
                    MessageStrings.InvalidCredentials);

            if (!user.IsActive)
                throw new InvalidOperationException(
                    MessageStrings.UserIsDisabled);

            return GenerateJwt(user);
        }

        private AuthenticationResponseDto GenerateJwt(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: Configuration["Jwt:Issuer"],
                audience: Configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new AuthenticationResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
    }
}