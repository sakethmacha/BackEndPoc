using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for theatre management operations
    /// </summary>
    public interface ITheatreService
    {
        /// <summary>Retrieves all active theatres</summary>
        Task<List<TheatreResponseDto>> GetTheatresAsync();

        /// <summary>Retrieves a theatre by ID with time slots</summary>
        Task<TheatreResponseDto> GetTheatreByIdAsync(Guid theatreId);

        /// <summary>Adds a new theatre with time slots</summary>
        Task AddTheatreAsync(CreateTheatreDto createTheatreDto, Guid superAdminId);

        /// <summary>Updates an existing theatre</summary>
        Task UpdateTheatreAsync(Guid theatreId, UpdateTheatreDto updateTheatreDto);

        /// <summary>Deletes a theatre (soft delete)</summary>
        Task DeleteTheatreAsync(Guid theatreId);
    }
}