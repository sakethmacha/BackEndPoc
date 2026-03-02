using MovieBooking.Domain.Entities;

namespace MovieBooking.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for showtime data access operations
    /// </summary>
    public interface IShowTimeRepository
    {
        /// <summary>Retrieves all active showtimes with related entities</summary>
        Task<List<ShowTime>> GetShowTimesAsync();

        /// <summary>Retrieves a showtime by ID with related entities</summary>
        Task<ShowTime> GetShowTimeByIdAsync(Guid showTimeId);

        /// <summary>Adds multiple showtimes</summary>
        Task AddShowTimesAsync(List<ShowTime> showTimes);

        /// <summary>Updates an existing showtime</summary>
        Task UpdateShowTimeAsync(ShowTime showTime);

        /// <summary>Soft deletes a showtime</summary>
        Task DeleteShowTimeAsync(ShowTime showTime);

        /// <summary>Checks if a showtime conflict exists for a screen</summary>
        Task<bool> ShowTimeConflictExistsAsync(Guid screenId, DateTime start, DateTime end);
    }
}