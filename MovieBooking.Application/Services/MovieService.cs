using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Constants;
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
        //public async Task AddMovieAsync(AddMovieDto addMovieDto)
        //{
        //    var movie = new Movie
        //    {
        //        MovieId = Guid.NewGuid(),
        //        Title = addMovieDto.Title,
        //        Description = addMovieDto.Description,
        //        DurationMinutes = addMovieDto.DurationMinutes,
        //        ReleaseDate = addMovieDto.ReleaseDate,
        //        PosterUrl = addMovieDto.PosterUrl,
        //        IsActive = true,
        //        CreatedAt = DateTime.UtcNow
        //    };
        //    await MovieRepository.AddMovieAsync(movie);
        //}
        public async Task AddMovieAsync(AddMovieDto addMovieDto)
        {
            // ADDED: file saving logic
            string? posterFileName = null;

            if (addMovieDto.PosterFile != null && addMovieDto.PosterFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                Directory.CreateDirectory(uploadsFolder); // creates folder if not exists

                posterFileName = Guid.NewGuid().ToString()
                                 + Path.GetExtension(addMovieDto.PosterFile.FileName);

                var filePath = Path.Combine(uploadsFolder, posterFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await addMovieDto.PosterFile.CopyToAsync(stream);
            }

            // YOUR EXACT CODE — only PosterUrl value changed
            var movie = new Movie
            {
                MovieId = Guid.NewGuid(),
                Title = addMovieDto.Title,
                Description = addMovieDto.Description,
                DurationMinutes = addMovieDto.DurationMinutes,
                ReleaseDate = addMovieDto.ReleaseDate,
                PosterUrl = posterFileName,  // CHANGED: saves filename like "abc123.jpg"
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await MovieRepository.AddMovieAsync(movie);
        }

        /// <inheritdoc/>
        //public async Task UpdateMovieAsync(Guid movieId, UpdateMovieDto updateMovieDto)
        //{
        //    var movie = await MovieRepository.GetMovieByIdAsync(movieId);
        //    movie.Title = updateMovieDto.Title;
        //    movie.Description = updateMovieDto.Description;
        //    movie.DurationMinutes = updateMovieDto.DurationMinutes;
        //    movie.ReleaseDate = updateMovieDto.ReleaseDate;
        //    movie.PosterUrl = updateMovieDto.PosterUrl;
        //    await MovieRepository.UpdateMovieAsync(movie);
        //}
        public async Task UpdateMovieAsync(Guid movieId, UpdateMovieDto updateMovieDto)
        {
            var movie = await MovieRepository.GetMovieByIdAsync(movieId);

            // ADDED: handle poster file
            if (updateMovieDto.PosterFile != null && updateMovieDto.PosterFile.Length > 0)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(movie.PosterUrl))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", movie.PosterUrl);
                    if (File.Exists(oldFilePath))
                        File.Delete(oldFilePath);
                }

                // Save new image
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                Directory.CreateDirectory(uploadsFolder);

                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(updateMovieDto.PosterFile.FileName);
                var newFilePath = Path.Combine(uploadsFolder, newFileName);

                using var stream = new FileStream(newFilePath, FileMode.Create);
                await updateMovieDto.PosterFile.CopyToAsync(stream);

                movie.PosterUrl = newFileName; // update to new filename
            }
            // if no new file uploaded → movie.PosterUrl stays as old value ✅

            // YOUR EXACT CODE — unchanged
            movie.Title = updateMovieDto.Title;
            movie.Description = updateMovieDto.Description;
            movie.DurationMinutes = updateMovieDto.DurationMinutes;
            movie.ReleaseDate = updateMovieDto.ReleaseDate;

            await MovieRepository.UpdateMovieAsync(movie);
        }
        /// <inheritdoc/>
        public async Task DeleteMovieAsync(Guid movieId)
        {
            var movie = await MovieRepository.GetMovieByIdAsync(movieId);
            var hasActiveShowTimes = await MovieRepository.MovieHasActiveShowTimesAsync(movieId);
            if (hasActiveShowTimes)
                throw new InvalidOperationException(MessageStrings.CannotDelete);
            await MovieRepository.DeleteMovieAsync(movie);
        }
    }
}