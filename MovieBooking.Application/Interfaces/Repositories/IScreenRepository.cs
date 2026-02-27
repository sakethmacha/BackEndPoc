using MovieBooking.Domain.Entities;

namespace MovieBooking.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for screen data access operations
    /// </summary>
    public interface IScreenRepository
    {
        /// <summary>
        /// Retrieves all active screens
        /// </summary>
        /// <returns>List of screens</returns>
        Task<List<Screen>> GetAllScreensAsync();

        /// <summary>
        /// Retrieves a screen by ID with seats
        /// </summary>
        /// <param name="screenId">Screen identifier</param>
        /// <returns>Screen entity with seats</returns>
        Task<Screen> GetScreenByIdAsync(Guid screenId);

        /// <summary>
        /// Retrieves all screens for a specific theatre
        /// </summary>
        /// <param name="theatreId">Theatre identifier</param>
        /// <returns>List of screens</returns>
        Task<List<Screen>> GetScreensByTheatreAsync(Guid theatreId);

        /// <summary>
        /// Adds a new screen
        /// </summary>
        /// <param name="screen">Screen entity</param>
        Task AddScreenAsync(Screen screen);

        /// <summary>
        /// Updates an existing screen
        /// </summary>
        /// <param name="screen">Screen entity</param>
        Task UpdateScreenAsync(Screen screen);

        /// <summary>
        /// Deletes a screen (soft delete by setting IsActive = false)
        /// </summary>
        /// <param name="screen">Screen entity</param>
        Task DeleteScreenAsync(Screen screen);

        /// <summary>
        /// Adds seats for a screen
        /// </summary>
        /// <param name="seats">List of seat entities</param>
        Task AddSeatsAsync(List<Seat> seats);

        /// <summary>
        /// Retrieves all seats for a screen
        /// </summary>
        /// <param name="screenId">Screen identifier</param>
        /// <returns>List of seats</returns>
        Task<List<Seat>> GetScreenSeatsAsync(Guid screenId);

        /// <summary>
        /// Deletes all seats for a screen
        /// </summary>
        /// <param name="screenId">Screen identifier</param>
        Task DeleteScreenSeatsAsync(Guid screenId);

        /// <summary>
        /// Checks if a screen has active showtimes
        /// </summary>
        /// <param name="screenId">Screen identifier</param>
        /// <returns>True if screen has active showtimes</returns>
        Task<bool> ScreenHasActiveShowTimesAsync(Guid screenId);
    }
}