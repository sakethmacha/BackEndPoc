using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for showtime management operations
    /// </summary>
    public interface IShowTimeService
    {
        /// <summary>
        /// Retrieves all showtimes
        /// </summary>
        /// <returns>List of showtimes</returns>
        Task<List<ShowTimeResponseDto>> GetShowTimesAsync();

        /// <summary>
        /// Retrieves a specific showtime by ID
        /// </summary>
        /// <param name="showTimeId">ShowTime identifier</param>
        /// <returns>ShowTime details</returns>
        Task<ShowTimeResponseDto> GetShowTimeByIdAsync(Guid showTimeId);

        /// <summary>
        /// Adds a new showtime
        /// </summary>
        /// <param name="createShowTimeDto">ShowTime data</param>
        Task AddShowTimeAsync(CreateShowTimeDto createShowTimeDto);

        /// <summary>
        /// Updates an existing showtime
        /// </summary>
        /// <param name="showTimeId">ShowTime identifier</param>
        /// <param name="updateShowTimeDto">Updated showtime data</param>
        Task UpdateShowTimeAsync(Guid showTimeId, UpdateShowTimeDto updateShowTimeDto);

        /// <summary>
        /// Deletes a showtime
        /// </summary>
        /// <param name="showTimeId">ShowTime identifier</param>
        Task DeleteShowTimeAsync(Guid showTimeId);
    }
}