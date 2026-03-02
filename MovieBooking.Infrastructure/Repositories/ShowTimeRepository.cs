using Microsoft.EntityFrameworkCore;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Domain.Entities;
using MovieBooking.Infrastructure.Persistence;

namespace MovieBooking.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for showtime data access operations
    /// </summary>
    public class ShowTimeRepository : IShowTimeRepository
    {
        private readonly MovieBookingDatabaseContext DbContext;

        /// <summary>Initializes a new instance of ShowTimeRepository</summary>
        public ShowTimeRepository(MovieBookingDatabaseContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<List<ShowTime>> GetShowTimesAsync()
            => await DbContext.ShowTimes
                .Where(st => st.IsActive)
                .Include(st => st.Movie)
                .Include(st => st.Theatre)
                .Include(st => st.Screen)
                .Include(st => st.Language)
                .AsNoTracking()
                .OrderBy(st => st.StartTime)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<ShowTime> GetShowTimeByIdAsync(Guid showTimeId)
        {
            var showTime = await DbContext.ShowTimes
                .Include(s => s.Movie)
                .Include(s => s.Theatre)
                .Include(s => s.Screen)
                .Include(s => s.Language)
                .FirstOrDefaultAsync(s => s.ShowTimeId == showTimeId);
            if (showTime == null)
                throw new InvalidOperationException("ShowTime not found");
            return showTime;
        }

        /// <inheritdoc/>
        public async Task AddShowTimesAsync(List<ShowTime> showTimes)
        {
            DbContext.ShowTimes.AddRange(showTimes);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateShowTimeAsync(ShowTime showTime)
        {
            DbContext.ShowTimes.Update(showTime);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteShowTimeAsync(ShowTime showTime)
        {
            showTime.IsActive = false;
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> ShowTimeConflictExistsAsync(Guid screenId, DateTime start, DateTime end)
            => await DbContext.ShowTimes.AnyAsync(st =>
                st.ScreenId == screenId &&
                st.StartTime < end &&
                start < st.EndTime);
    }
}