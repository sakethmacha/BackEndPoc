using Microsoft.EntityFrameworkCore;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Domain.Entities;
using MovieBooking.Infrastructure.Persistence;

namespace MovieBooking.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MovieBookingDatabaseContext DbContext;

        public UserRepository(MovieBookingDatabaseContext Context)
        {
            this.DbContext = Context;
        }

        public async Task<User?> GetByEmailAsync(string Email)
        {
            return await DbContext.Users.FirstOrDefaultAsync(x => x.Email == Email);
        }

        public async Task AddAsync(User User)
        {
            DbContext.Users.Add(User);
            await DbContext.SaveChangesAsync();
        }
    }

}
