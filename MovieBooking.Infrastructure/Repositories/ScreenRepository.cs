using Microsoft.EntityFrameworkCore;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Domain.Constants;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;
using MovieBooking.Infrastructure.Persistence;

namespace MovieBooking.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for screen data access operations
    /// </summary>
    public class ScreenRepository : IScreenRepository
    {
        private readonly MovieBookingDatabaseContext DbContext;

        /// <summary>Initializes a new instance of ScreenRepository</summary>
        public ScreenRepository(MovieBookingDatabaseContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<List<Screen>> GetScreensAsync()
            => await DbContext.Screens
                .Where(s => s.IsActive)
                .AsNoTracking()
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<Screen> GetScreenByIdAsync(Guid screenId)
        {
            var screen = await DbContext.Screens
                .Include(s => s.Seats)
                .FirstOrDefaultAsync(s => s.ScreenId == screenId);
            if (screen == null)
                throw new InvalidOperationException(MessageStrings.ScreenNotFound);
            return screen;
        }

        /// <inheritdoc/>
        public async Task<List<Screen>> GetByTheatreIdAsync(Guid theatreId)
            => await DbContext.Screens
                .Where(s => s.TheatreId == theatreId && s.IsActive)
                .OrderBy(s => s.ScreenName)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task AddScreenAsync(Screen screen)
        {
            DbContext.Screens.Add(screen);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateScreenAsync(Screen screen)
        {
            DbContext.Screens.Update(screen);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteScreenAsync(Screen screen)
        {
            if (await ScreenHasActiveShowTimesAsync(screen.ScreenId))
                throw new InvalidOperationException(MessageStrings.CannotDeactivateScreenWithActiveShowTimes);
            screen.IsActive = false;
            foreach (var seat in screen.Seats)
                seat.IsActive = false;
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task AddSeatsAsync(List<Seat> seats)
        {
            DbContext.Seats.AddRange(seats);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<List<Seat>> GetScreenSeatsAsync(Guid screenId)
            => await DbContext.Seats
                .Where(s => s.ScreenId == screenId)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task DeleteScreenSeatsAsync(Guid screenId)
        {
            var seats = await DbContext.Seats.Where(s => s.ScreenId == screenId).ToListAsync();
            DbContext.Seats.RemoveRange(seats);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> ScreenHasActiveShowTimesAsync(Guid screenId)
            => await DbContext.ShowTimes.AnyAsync(st => st.ScreenId == screenId && st.IsActive);

        /// <inheritdoc/>
        public async Task ApproveScreenAsync(Guid screenId)
        {
            var screen = await DbContext.Screens.FindAsync(screenId);
            screen.IsActive = true;
            screen.ApprovalStatus = ApprovalStatus.APPROVED;
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task RejectScreenAsync(Guid screenId)
        {
            var screen = await DbContext.Screens.FindAsync(screenId);
            screen.ApprovalStatus = ApprovalStatus.REJECTED;
            await DbContext.SaveChangesAsync();
        }
    }
}