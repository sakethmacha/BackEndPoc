using Microsoft.EntityFrameworkCore;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Domain.Constants;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;
using MovieBooking.Infrastructure.Persistence;

namespace MovieBooking.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for theatre data access operations
    /// </summary>
    public class TheatreRepository : ITheatreRepository
    {
        private readonly MovieBookingDatabaseContext DbContext;

        /// <summary>Initializes a new instance of TheatreRepository</summary>
        public TheatreRepository(MovieBookingDatabaseContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<List<Theatre>> GetTheatresAsync()
            => await DbContext.Theatres
                .Where(t => t.IsActive)
                .AsNoTracking()
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<Theatre> GetTheatreByIdAsync(Guid theatreId)
        {
            var theatre = await DbContext.Theatres
                .Include(t => t.TimeSlots)
                .FirstOrDefaultAsync(t => t.TheatreId == theatreId);

            if (theatre == null)
                throw new InvalidOperationException(
                    MessageStrings.TheatreNotFound);

            return theatre;
        }

        /// <inheritdoc/>
        public async Task AddTheatreWithTimeSlotsAsync(
            Theatre theatre,
            List<TheatreTimeSlot> timeSlots)
        {
            DbContext.Theatres.Add(theatre);
            DbContext.TheatreTimeSlots.AddRange(timeSlots);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateTheatreAsync(Theatre theatre)
        {
            DbContext.Theatres.Update(theatre);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteTheatreAsync(Theatre theatre)
        {
            if (await TheatreHasActiveScreensAsync(theatre.TheatreId))
                throw new InvalidOperationException(
                    MessageStrings.CannotDeactivateTheatreWithActiveScreens);

            theatre.IsActive = false;
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<List<TheatreTimeSlot>> GetTimeSlotsByTheatreAsync(Guid theatreId)
            => await DbContext.TheatreTimeSlots
                .Where(t => t.TheatreId == theatreId && t.IsActive)
                .OrderBy(t => t.StartTime)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<List<TheatreTimeSlot>> GetTheatreTimeSlotsAsync(Guid theatreId)
            => await DbContext.TheatreTimeSlots
                .Where(ts => ts.TheatreId == theatreId)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task DeleteTheatreTimeSlotsAsync(Guid theatreId)
        {
            var timeSlots = await DbContext.TheatreTimeSlots
                .Where(ts => ts.TheatreId == theatreId)
                .ToListAsync();

            DbContext.TheatreTimeSlots.RemoveRange(timeSlots);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> TheatreHasActiveScreensAsync(Guid theatreId)
            => await DbContext.Screens
                .AnyAsync(s => s.TheatreId == theatreId && s.IsActive);

        /// <inheritdoc/>
        public async Task ApproveTheatreAsync(Guid theatreId)
        {
            var theatre = await DbContext.Theatres.FindAsync(theatreId);

            theatre.IsActive = true;
            theatre.ApprovalStatus = ApprovalStatus.APPROVED;

            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task RejectTheatreAsync(Guid theatreId)
        {
            var theatre = await DbContext.Theatres.FindAsync(theatreId);

            theatre.ApprovalStatus = ApprovalStatus.REJECTED;

            await DbContext.SaveChangesAsync();
        }
    }
}