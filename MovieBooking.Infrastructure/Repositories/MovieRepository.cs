using Microsoft.EntityFrameworkCore;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Domain.Entities;
using MovieBooking.Infrastructure.Persistence;

namespace MovieBooking.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for movie data access operations
    /// </summary>
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieBookingDatabaseContext DbContext;

        /// <summary>Initializes a new instance of MovieRepository</summary>
        public MovieRepository(MovieBookingDatabaseContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<List<Movie>> GetAllAsync()
            => await DbContext.Movies
                .Where(m => m.IsActive)
                .AsNoTracking()
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();

        /// <inheritdoc/>
        public Task<Movie> GetMovieByIdAsync(Guid movieId)
            => DbContext.Movies.FindAsync(movieId).AsTask();

        /// <inheritdoc/>
        public async Task AddMovieAsync(Movie movie)
        {
            DbContext.Movies.Add(movie);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateMovieAsync(Movie movie)
        {
            DbContext.Movies.Update(movie);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteMovieAsync(Movie movie)
        {
            if (await MovieHasActiveShowTimesAsync(movie.MovieId))
                throw new InvalidOperationException("Cannot deactivate movie while active showtimes exist.");
            movie.IsActive = false;
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> MovieHasActiveShowTimesAsync(Guid movieId)
            => await DbContext.ShowTimes.AnyAsync(st => st.MovieId == movieId && st.IsActive);
    }
}