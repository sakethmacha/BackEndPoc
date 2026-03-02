using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;

namespace MovieBooking.Application.Services
{
    /// <summary>
    /// Service implementation for admin user management operations
    /// </summary>
    public class AdminManagementService : IAdminManagementService
    {
        private readonly IAdminManagementRepository AdminManagementRepository;

        /// <summary>Initializes a new instance of AdminManagementService</summary>
        public AdminManagementService(IAdminManagementRepository adminManagementRepository)
        {
            AdminManagementRepository = adminManagementRepository;
        }

        /// <inheritdoc/>
        public async Task<List<AdminDto>> GetAdminsAsync()
            => await AdminManagementRepository.GetAdminsAsync();

        /// <inheritdoc/>
        public async Task<AdminDto> GetAdminByIdAsync(Guid adminId)
        {
            var admin = await AdminManagementRepository.GetUserByIdAsync(adminId);
            if (admin.Role != UserRole.Admin)
                throw new InvalidOperationException("User is not an admin");

            return new AdminDto
            {
                UserId = admin.UserId,
                Name = admin.Name,
                Email = admin.Email,
                IsActive = admin.IsActive
            };
        }

        /// <inheritdoc/>
        public async Task CreateAdminAsync(CreateAdminDto createAdminDto)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Name = createAdminDto.Name,
                Email = createAdminDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(createAdminDto.Password),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await AdminManagementRepository.CreateAdminAsync(user);
        }

        /// <inheritdoc/>
        public async Task ToggleAdminAsync(Guid adminId)
        {
            var admin = await AdminManagementRepository.GetUserByIdAsync(adminId);
            admin.IsActive = !admin.IsActive;
            await AdminManagementRepository.UpdateUserAsync(admin);
        }

        /// <inheritdoc/>
        public async Task UpdateAdminAsync(Guid adminId, UpdateAdminDto updateAdminDto)
        {
            var admin = await AdminManagementRepository.GetUserByIdAsync(adminId);
            if (admin.Role != UserRole.Admin)
                throw new InvalidOperationException("User is not an admin");

            admin.Name = updateAdminDto.Name;
            admin.Email = updateAdminDto.Email;
            if (!string.IsNullOrWhiteSpace(updateAdminDto.Password))
                admin.Password = BCrypt.Net.BCrypt.HashPassword(updateAdminDto.Password);

            await AdminManagementRepository.UpdateUserAsync(admin);
        }

        /// <inheritdoc/>
        public async Task DeleteAdminAsync(Guid adminId)
        {
            var admin = await AdminManagementRepository.GetUserByIdAsync(adminId);
            if (admin.Role != UserRole.Admin)
                throw new InvalidOperationException("User is not an admin");

            var hasActiveTheatres = await AdminManagementRepository.AdminHasActiveTheatresAsync(adminId);
            if (hasActiveTheatres)
                throw new InvalidOperationException("Cannot delete admin with active theatres. Please reassign or delete theatres first.");

            await AdminManagementRepository.DeleteAdminAsync(admin);
        }
    }
}