using MovieBooking.Domain.Entities;

namespace MovieBooking.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for movie data access operations
    /// </summary>
    public interface IMovieRepository
    {
        /// <summary>
        /// Retrieves all active movies
        /// </summary>
        /// <returns>List of movies</returns>
        Task<List<Movie>> GetAllMoviesAsync();

        /// <summary>
        /// Retrieves a movie by ID
        /// </summary>
        /// <param name="movieId">Movie identifier</param>
        /// <returns>Movie entity</returns>
        Task<Movie> GetMovieByIdAsync(Guid movieId);

        /// <summary>
        /// Adds a new movie
        /// </summary>
        /// <param name="movie">Movie entity</param>
        Task AddMovieAsync(Movie movie);

        /// <summary>
        /// Updates an existing movie
        /// </summary>
        /// <param name="movie">Movie entity</param>
        Task UpdateMovieAsync(Movie movie);

        /// <summary>
        /// Deletes a movie (soft delete by setting IsActive = false)
        /// </summary>
        /// <param name="movie">Movie entity</param>
        Task DeleteMovieAsync(Movie movie);

        /// <summary>
        /// Checks if a movie has active showtimes
        /// </summary>
        /// <param name="movieId">Movie identifier</param>
        /// <returns>True if movie has active showtimes</returns>
        Task<bool> MovieHasActiveShowTimesAsync(Guid movieId);
    }
}