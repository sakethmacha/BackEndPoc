using Microsoft.Extensions.Configuration;
using MovieBooking.Application.DTOs.Authentication;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
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
            IUserRepository UserRepository,
            IConfiguration Configuration)
        {
            this.UserRepository = UserRepository;
            this.Configuration = Configuration;
        }

        public async Task<AuthenticationResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            if (await UserRepository.GetByEmailAsync(dto.Email) != null)
                throw new Exception("Email already registered");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await UserRepository.AddAsync(user);

            return GenerateJwt(user);
        }


        public async Task<AuthenticationResponseDto> LoginAsync(LoginRequestDto Dto)
        {
            var User = await UserRepository.GetByEmailAsync(Dto.Email)
                ?? throw new Exception("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(Dto.Password, User.Password))
                throw new Exception("Invalid credentials");

            if (!User.IsActive)
                throw new Exception("User is disabled");

            return GenerateJwt(User);
        }

        private AuthenticationResponseDto GenerateJwt(User User)
        {
            var Key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]!));

            var Credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            //var Claims = new[]
            //{
            //new Claim(JwtRegisteredClaimNames.Sub, User.UserId.ToString()),
            //new Claim(JwtRegisteredClaimNames.Email, User.Email),
            //new Claim(ClaimTypes.Role, User.Role.ToString())
            //};
            var Claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, User.UserId.ToString()), // 🔥 CHANGE
                new Claim(ClaimTypes.Email, User.Email),
                new Claim(ClaimTypes.Role, User.Role.ToString())
            };

            var Token = new JwtSecurityToken(
                issuer: Configuration["Jwt:Issuer"],
                audience: Configuration["Jwt:Audience"],
                claims: Claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: Credentials
            );

            return new AuthenticationResponseDto
            {
                UserId = User.UserId,
                Name = User.Name,
                Email = User.Email,
                Role = User.Role.ToString(),
                Token = new JwtSecurityTokenHandler().WriteToken(Token)
            };
        }
    }

}
