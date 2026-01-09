using BCrypt.Net;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;
using MovieBooking.Infrastructure.Persistence;

namespace MovieBooking.Infrastructure.Seed
{
    public static class SuperAdminSeeder
    {
        public static async Task SeedAsync(MovieBookingDatabaseContext Context)
        {
            //// If SUPER_ADMIN already exists, do nothing
            if (Context.Users.Any(x => x.Role == UserRole.SuperAdmin))
                return;

            var superAdmin = new User
            {
                UserId = Guid.NewGuid(),
                Name = "System Super Admin",
                Email = "superadmin@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@1369"),
                Role = UserRole.SuperAdmin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            Context.Users.Add(superAdmin);
            await Context.SaveChangesAsync();
        }
    }
}
