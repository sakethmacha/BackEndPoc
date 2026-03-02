using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;

namespace MovieBooking.Application.Services
{
    /// <summary>
    /// Service implementation for movie management operations
    /// </summary>
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository MovieRepository;

        /// <summary>Initializes a new instance of MovieService</summary>
        public MovieService(IMovieRepository movieRepository)
        {
            MovieRepository = movieRepository;
        }

        /// <inheritdoc/>
        public async Task<List<MovieResponse>> GetMoviesAsync()
        {
            var movies = await MovieRepository.GetAllAsync();
            return movies.Select(m => new MovieResponse
            {
                MovieId = m.MovieId,
                Title = m.Title,
                DurationMinutes = m.DurationMinutes,
                ReleaseDate = m.ReleaseDate,
                IsActive = m.IsActive,
                PosterUrl = m.PosterUrl
            }).ToList();
        }

        /// <inheritdoc/>
        public async Task<MovieResponse> GetMovieByIdAsync(Guid movieId)
        {
            var movie = await MovieRepository.GetMovieByIdAsync(movieId);
            return new MovieResponse
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                Description = movie.Description,
                DurationMinutes = movie.DurationMinutes,
                ReleaseDate = movie.ReleaseDate,
                PosterUrl = movie.PosterUrl,
                IsActive = movie.IsActive
            };
        }

        /// <inheritdoc/>
        public async Task AddMovieAsync(AddMovieDto addMovieDto)
        {
            var movie = new Movie
            {
                MovieId = Guid.NewGuid(),
                Title = addMovieDto.Title,
                Description = addMovieDto.Description,
                DurationMinutes = addMovieDto.DurationMinutes,
                ReleaseDate = addMovieDto.ReleaseDate,
                PosterUrl = addMovieDto.PosterUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await MovieRepository.AddMovieAsync(movie);
        }

        /// <inheritdoc/>
        public async Task ToggleMovieAsync(Guid movieId)
        {
            var movie = await MovieRepository.GetMovieByIdAsync(movieId);
            movie.IsActive = !movie.IsActive;
            await MovieRepository.UpdateMovieAsync(movie);
        }

        /// <inheritdoc/>
        public async Task UpdateMovieAsync(Guid movieId, UpdateMovieDto updateMovieDto)
        {
            var movie = await MovieRepository.GetMovieByIdAsync(movieId);
            movie.Title = updateMovieDto.Title;
            movie.Description = updateMovieDto.Description;
            movie.DurationMinutes = updateMovieDto.DurationMinutes;
            movie.ReleaseDate = updateMovieDto.ReleaseDate;
            movie.PosterUrl = updateMovieDto.PosterUrl;
            await MovieRepository.UpdateMovieAsync(movie);
        }

        /// <inheritdoc/>
        public async Task DeleteMovieAsync(Guid movieId)
        {
            var movie = await MovieRepository.GetMovieByIdAsync(movieId);
            var hasActiveShowTimes = await MovieRepository.MovieHasActiveShowTimesAsync(movieId);
            if (hasActiveShowTimes)
                throw new InvalidOperationException("Cannot delete movie with active showtimes. Please deactivate or delete showtimes first.");
            await MovieRepository.DeleteMovieAsync(movie);
        }
    }
}