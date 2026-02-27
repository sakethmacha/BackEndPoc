using Microsoft.AspNetCore.Http;
using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for movie management operations
    /// </summary>
    public interface IMovieService
    {
        /// <summary>
        /// Retrieves all movies
        /// </summary>
        /// <returns>List of movies</returns>
        Task<List<MovieResponse>> GetMoviesAsync();

        /// <summary>
        /// Retrieves a specific movie by ID
        /// </summary>
        /// <param name="movieId">Movie identifier</param>
        /// <returns>Movie details</returns>
        Task<MovieResponse> GetMovieByIdAsync(Guid movieId);

        /// <summary>
        /// Retrieves movie poster image data
        /// </summary>
        /// <param name="movieId">Movie identifier</param>
        /// <returns>Tuple containing image data and content type</returns>
        Task<(byte[] imageData, string contentType)> GetMoviePosterAsync(Guid movieId);

        /// <summary>
        /// Adds a new movie
        /// </summary>
        /// <param name="addMovieDto">Movie data</param>
        /// <param name="posterImage">Optional poster image file</param>
        Task AddMovieAsync(AddMovieDto addMovieDto, IFormFile? posterImage = null);

        /// <summary>
        /// Updates an existing movie
        /// </summary>
        /// <param name="movieId">Movie identifier</param>
        /// <param name="updateMovieDto">Updated movie data</param>
        /// <param name="posterImage">Optional new poster image file</param>
        Task UpdateMovieAsync(Guid movieId, UpdateMovieDto updateMovieDto, IFormFile? posterImage = null);

        /// <summary>
        /// Deletes a movie
        /// </summary>
        /// <param name="movieId">Movie identifier</param>
        Task DeleteMovieAsync(Guid movieId);
    }
}