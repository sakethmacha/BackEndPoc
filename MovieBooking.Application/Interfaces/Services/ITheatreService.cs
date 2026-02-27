using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for theatre management operations
    /// </summary>
    public interface ITheatreService
    {
        /// <summary>
        /// Retrieves all theatres
        /// </summary>
        /// <returns>List of theatres</returns>
        Task<List<TheatreResponseDto>> GetTheatresAsync();

        /// <summary>
        /// Retrieves a specific theatre by ID
        /// </summary>
        /// <param name="theatreId">Theatre identifier</param>
        /// <returns>Theatre details with time slots</returns>
        Task<TheatreResponseDto> GetTheatreByIdAsync(Guid theatreId);

        /// <summary>
        /// Adds a new theatre
        /// </summary>
        /// <param name="createTheatreDto">Theatre data</param>
        /// <param name="superAdminId">ID of the super admin creating the theatre</param>
        Task AddTheatreAsync(CreateTheatreDto createTheatreDto, Guid superAdminId);

        /// <summary>
        /// Updates an existing theatre
        /// </summary>
        /// <param name="theatreId">Theatre identifier</param>
        /// <param name="updateTheatreDto">Updated theatre data</param>
        Task UpdateTheatreAsync(Guid theatreId, UpdateTheatreDto updateTheatreDto);

        /// <summary>
        /// Deletes a theatre
        /// </summary>
        /// <param name="theatreId">Theatre identifier</param>
        Task DeleteTheatreAsync(Guid theatreId);
    }
}