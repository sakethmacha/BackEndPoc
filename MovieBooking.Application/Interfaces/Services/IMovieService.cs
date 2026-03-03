using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for movie management operations
    /// </summary>
    public interface IMovieService
    {
        /// <summary>Retrieves all active movies</summary>
        Task<List<MovieResponse>> GetMoviesAsync();

        /// <summary>Retrieves a movie by ID</summary>
        Task<MovieResponse> GetMovieByIdAsync(Guid movieId);

        /// <summary>Adds a new movie</summary>
        Task AddMovieAsync(AddMovieDto addMovieDto);

        /// <summary>Updates an existing movie</summary>
        Task UpdateMovieAsync(Guid movieId, UpdateMovieDto updateMovieDto);

        /// <summary>Deletes a movie (soft delete)</summary>
        Task DeleteMovieAsync(Guid movieId);
    }
}