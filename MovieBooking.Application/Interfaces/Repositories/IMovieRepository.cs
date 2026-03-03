using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Domain.Entities;

namespace MovieBooking.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for movie data access operations
    /// </summary>
    public interface IMovieRepository
    {
        /// <summary>Retrieves all active movies ordered by release date</summary>
        Task<List<Movie>> GetAllAsync();

        /// <summary>Retrieves a movie by ID</summary>
        Task<Movie> GetMovieByIdAsync(Guid movieId);

        /// <summary>Adds a new movie</summary>
        Task AddMovieAsync(Movie movie);

        /// <summary>Updates an existing movie</summary>
        Task UpdateMovieAsync(Movie movie);

        /// <summary>Soft deletes a movie</summary>
        Task DeleteMovieAsync(Movie movie);

        /// <summary>Checks if movie has active showtimes</summary>
        Task<bool> MovieHasActiveShowTimesAsync(Guid movieId);
    }
}