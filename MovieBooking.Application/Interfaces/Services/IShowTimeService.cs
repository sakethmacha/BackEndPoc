using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for showtime management operations
    /// </summary>
    public interface IShowTimeService
    {
        /// <summary>Retrieves all active showtimes</summary>
        Task<List<ShowTimeResponseDto>> GetShowTimesAsync();

        /// <summary>Retrieves a showtime by ID</summary>
        Task<ShowTimeResponseDto> GetShowTimeByIdAsync(Guid showTimeId);

        /// <summary>Adds new showtimes for all theatre time slots</summary>
        Task AddShowTimeAsync(CreateShowTimeDto createShowTimeDto);

        /// <summary>Updates an existing showtime</summary>
        Task UpdateShowTimeAsync(Guid showTimeId, UpdateShowTimeDto updateShowTimeDto);

        /// <summary>Deletes a showtime (soft delete)</summary>
        Task DeleteShowTimeAsync(Guid showTimeId);
    }
}