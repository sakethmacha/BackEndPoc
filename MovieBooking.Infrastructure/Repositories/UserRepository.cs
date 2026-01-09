using Microsoft.EntityFrameworkCore;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Domain.Entities;
using MovieBooking.Infrastructure.Persistence;

namespace MovieBooking.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MovieBookingDatabaseContext Context;

        public UserRepository(MovieBookingDatabaseContext Context)
        {
            this.Context = Context;
        }

        public async Task<User?> GetByEmailAsync(string Email)
        {
            return await Context.Users.FirstOrDefaultAsync(x => x.Email == Email);
        }

        public async Task AddAsync(User User)
        {
            Context.Users.Add(User);
            await Context.SaveChangesAsync();
        }
    }

}
