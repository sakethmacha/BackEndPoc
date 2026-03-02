using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for screen management operations
    /// </summary>
    public interface IScreenService
    {
        /// <summary>Retrieves all active screens</summary>
        Task<List<ScreenResponseDto>> GetScreensAsync();

        /// <summary>Retrieves a screen by ID with seat layout</summary>
        Task<CreateScreenRequest> GetScreenByIdAsync(Guid screenId);

        /// <summary>Retrieves screens by theatre ID</summary>
        Task<List<ScreenResponseDto>> GetScreensByTheatreAsync(Guid theatreId);

        /// <summary>Adds a new screen with seats</summary>
        Task AddScreenAsync(CreateScreenRequest createScreenRequest);

        /// <summary>Updates an existing screen</summary>
        Task UpdateScreenAsync(Guid screenId, UpdateScreenDto updateScreenDto);

        /// <summary>Deletes a screen (soft delete)</summary>
        Task DeleteScreenAsync(Guid screenId);
    }
}