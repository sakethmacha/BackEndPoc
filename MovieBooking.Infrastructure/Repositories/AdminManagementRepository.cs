using Microsoft.EntityFrameworkCore;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;
using MovieBooking.Infrastructure.Persistence;

namespace MovieBooking.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for admin user data access operations
    /// </summary>
    public class AdminManagementRepository : IAdminManagementRepository
    {
        private readonly MovieBookingDatabaseContext DbContext;

        /// <summary>Initializes a new instance of AdminManagementRepository</summary>
        public AdminManagementRepository(MovieBookingDatabaseContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<List<AdminDto>> GetAdminsAsync()
            => await DbContext.Users
                .Where(u => u.Role == UserRole.Admin)
                .Select(u => new AdminDto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    IsActive = u.IsActive
                }).ToListAsync();

        /// <inheritdoc/>
        public Task<User> GetUserByIdAsync(Guid userId)
            => DbContext.Users.FindAsync(userId).AsTask();

        /// <inheritdoc/>
        public async Task CreateAdminAsync(User user)
        {
            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateUserAsync(User user)
        {
            DbContext.Users.Update(user);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteAdminAsync(User admin)
        {
            if (await AdminHasActiveTheatresAsync(admin.UserId))
                throw new InvalidOperationException("Cannot deactivate admin while active theatres exist.");
            admin.IsActive = false;
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> AdminHasActiveTheatresAsync(Guid adminId)
            => await DbContext.Theatres.AnyAsync(t => t.CreatedBy == adminId && t.IsActive);
    }
}