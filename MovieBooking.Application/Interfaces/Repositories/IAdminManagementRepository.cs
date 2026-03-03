using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Domain.Entities;

namespace MovieBooking.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for admin user data access operations
    /// </summary>
    public interface IAdminManagementRepository
    {
        /// <summary>Retrieves all admin users as DTOs</summary>
        Task<List<AdminDto>> GetAdminsAsync();

        /// <summary>Retrieves a user by ID</summary>
        Task<User> GetUserByIdAsync(Guid userId);

        /// <summary>Creates a new admin user</summary>
        Task CreateAdminAsync(User user);

        /// <summary>Updates an existing user</summary>
        Task UpdateUserAsync(User user);

        /// <summary>Soft deletes an admin</summary>
        Task DeleteAdminAsync(User admin);

        /// <summary>Checks if admin has active theatres</summary>
        Task<bool> AdminHasActiveTheatresAsync(Guid adminId);
    }
}