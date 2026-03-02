using MovieBooking.Domain.Entities;

namespace MovieBooking.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for screen data access operations
    /// </summary>
    public interface IScreenRepository
    {
        /// <summary>Retrieves all active screens</summary>
        Task<List<Screen>> GetScreensAsync();

        /// <summary>Retrieves a screen by ID with seats</summary>
        Task<Screen> GetScreenByIdAsync(Guid screenId);

        /// <summary>Retrieves screens by theatre ID</summary>
        Task<List<Screen>> GetByTheatreIdAsync(Guid theatreId);

        /// <summary>Adds a new screen</summary>
        Task AddScreenAsync(Screen screen);

        /// <summary>Updates an existing screen</summary>
        Task UpdateScreenAsync(Screen screen);

        /// <summary>Soft deletes a screen</summary>
        Task DeleteScreenAsync(Screen screen);

        /// <summary>Adds seats for a screen</summary>
        Task AddSeatsAsync(List<Seat> seats);

        /// <summary>Retrieves all seats for a screen</summary>
        Task<List<Seat>> GetScreenSeatsAsync(Guid screenId);

        /// <summary>Deletes all seats for a screen</summary>
        Task DeleteScreenSeatsAsync(Guid screenId);

        /// <summary>Checks if screen has active showtimes</summary>
        Task<bool> ScreenHasActiveShowTimesAsync(Guid screenId);

        /// <summary>Approves a screen</summary>
        Task ApproveScreenAsync(Guid screenId);

        /// <summary>Rejects a screen</summary>
        Task RejectScreenAsync(Guid screenId);
    }
}