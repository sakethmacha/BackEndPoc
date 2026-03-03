using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for admin user management operations
    /// </summary>
    public interface IAdminManagementService
    {
        /// <summary>Retrieves all admin users</summary>
        Task<List<AdminDto>> GetAdminsAsync();

        /// <summary>Retrieves an admin by ID</summary>
        Task<AdminDto> GetAdminByIdAsync(Guid adminId);

        /// <summary>Creates a new admin user</summary>
        Task CreateAdminAsync(CreateAdminDto createAdminDto);

        /// <summary>Toggles admin active status</summary>
        Task ToggleAdminAsync(Guid adminId);

        /// <summary>Updates an existing admin</summary>
        Task UpdateAdminAsync(Guid adminId, UpdateAdminDto updateAdminDto);

        /// <summary>Deletes an admin (soft delete)</summary>
        Task DeleteAdminAsync(Guid adminId);
    }
}