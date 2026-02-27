using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for screen management operations
    /// </summary>
    public interface IScreenService
    {
        /// <summary>
        /// Retrieves all screens
        /// </summary>
        /// <returns>List of screens</returns>
        Task<List<ScreenResponseDto>> GetScreensAsync();

        /// <summary>
        /// Retrieves a specific screen by ID
        /// </summary>
        /// <param name="screenId">Screen identifier</param>
        /// <returns>Screen details with seat layout</returns>
        Task<CreateScreenRequest> GetScreenByIdAsync(Guid screenId);

        /// <summary>
        /// Retrieves all screens for a specific theatre
        /// </summary>
        /// <param name="theatreId">Theatre identifier</param>
        /// <returns>List of screens for the theatre</returns>
        Task<List<ScreenResponseDto>> GetScreensByTheatreAsync(Guid theatreId);

        /// <summary>
        /// Adds a new screen
        /// </summary>
        /// <param name="createScreenRequest">Screen data with seat configuration</param>
        Task AddScreenAsync(CreateScreenRequest createScreenRequest);

        /// <summary>
        /// Updates an existing screen
        /// </summary>
        /// <param name="screenId">Screen identifier</param>
        /// <param name="updateScreenDto">Updated screen data</param>
        Task UpdateScreenAsync(Guid screenId, UpdateScreenDto updateScreenDto);

        /// <summary>
        /// Deletes a screen
        /// </summary>
        /// <param name="screenId">Screen identifier</param>
        Task DeleteScreenAsync(Guid screenId);
    }
}