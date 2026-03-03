using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Constants;

namespace MovieBooking.Api.Controllers
{
    /// <summary>
    /// Controller for managing movie operations
    /// </summary>
    [ApiController]
    [Route("api/movie")]
    [Authorize(Roles = "SuperAdmin")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService MovieService;

        /// <summary>Initializes a new instance of MovieController</summary>
        public MovieController(IMovieService movieService)
        {
            MovieService = movieService;
        }

        /// <summary>Retrieves all active movies</summary>
        [HttpGet]
        public async Task<IActionResult> GetMovies()
        {
            try
            {
                var movies = await MovieService.GetMoviesAsync();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = MessageStrings.ErrorRetrievingMovies,
                    error = ex.Message
                });
            }
        }

        /// <summary>Retrieves a movie by ID</summary>
        [HttpGet("{movieId}")]
        public async Task<IActionResult> GetMovieById(Guid movieId)
        {
            if (movieId == Guid.Empty)
                return BadRequest(new { message = MessageStrings.InvalidMovieId });

            try
            {
                var movie = await MovieService.GetMovieByIdAsync(movieId);
                return Ok(movie);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = MessageStrings.ErrorRetrievingMovie,
                    error = ex.Message
                });
            }
        }

        /// <summary>Adds a new movie</summary>
        [HttpPost]
        public async Task<IActionResult> AddMovie(AddMovieDto addMovieDto)
        {
            try
            {
                await MovieService.AddMovieAsync(addMovieDto);

                return Ok(new
                {
                    message = MessageStrings.MovieAddedSuccessfully
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = MessageStrings.ErrorAddingMovie,
                    error = ex.Message
                });
            }
        }

        /// <summary>Toggles movie active status</summary>
        [HttpPut("{movieId}/toggle")]
        public async Task<IActionResult> ToggleMovie(Guid movieId)
        {
            if (movieId == Guid.Empty)
                return BadRequest(new { message = MessageStrings.InvalidMovieId });

            try
            {
                await MovieService.ToggleMovieAsync(movieId);

                return Ok(new
                {
                    message = MessageStrings.MovieStatusToggledSuccessfully
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = MessageStrings.ErrorTogglingMovie,
                    error = ex.Message
                });
            }
        }

        /// <summary>Updates an existing movie</summary>
        [HttpPut("{movieId}")]
        public async Task<IActionResult> UpdateMovie(Guid movieId, [FromBody] UpdateMovieDto updateMovieDto)
        {
            if (movieId == Guid.Empty)
                return BadRequest(new { message = MessageStrings.InvalidMovieId });

            try
            {
                await MovieService.UpdateMovieAsync(movieId, updateMovieDto);

                return Ok(new
                {
                    message = MessageStrings.MovieUpdatedSuccessfully
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = MessageStrings.ErrorUpdatingMovie,
                    error = ex.Message
                });
            }
        }

        /// <summary>Deletes a movie</summary>
        [HttpDelete("{movieId}")]
        public async Task<IActionResult> DeleteMovie(Guid movieId)
        {
            if (movieId == Guid.Empty)
                return BadRequest(new { message = MessageStrings.InvalidMovieId });

            try
            {
                await MovieService.DeleteMovieAsync(movieId);

                return Ok(new
                {
                    message = MessageStrings.MovieDeletedSuccessfully
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = MessageStrings.ErrorDeletingMovie,
                    error = ex.Message
                });
            }
        }
    }
}