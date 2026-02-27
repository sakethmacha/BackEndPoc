using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for admin user management operations
    /// </summary>
    public interface IAdminManagementService
    {
        /// <summary>
        /// Retrieves all admin users
        /// </summary>
        /// <returns>List of admin users</returns>
        Task<List<AdminDto>> GetAdminsAsync();

        /// <summary>
        /// Retrieves a specific admin by ID
        /// </summary>
        /// <param name="adminId">Admin identifier</param>
        /// <returns>Admin details</returns>
        Task<AdminDto> GetAdminByIdAsync(Guid adminId);

        /// <summary>
        /// Creates a new admin user
        /// </summary>
        /// <param name="createAdminDto">Admin creation data</param>
        Task CreateAdminAsync(CreateAdminDto createAdminDto);

        /// <summary>
        /// Updates an existing admin user
        /// </summary>
        /// <param name="adminId">Admin identifier</param>
        /// <param name="updateAdminDto">Updated admin data</param>
        Task UpdateAdminAsync(Guid adminId, UpdateAdminDto updateAdminDto);

        /// <summary>
        /// Deletes an admin user
        /// </summary>
        /// <param name="adminId">Admin identifier</param>
        Task DeleteAdminAsync(Guid adminId);
    }
}