using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Services;

namespace MovieBooking.Api.Controllers
{
    /// <summary>
    /// Controller for managing movie operations
    /// </summary>
    [ApiController]
    [Route("api/superadmin/movies")]
    [Authorize(Roles = "SuperAdmin")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        /// <summary>
        /// Initializes a new instance of the MovieController
        /// </summary>
        /// <param name="movieService">Movie service instance</param>
        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        /// <summary>
        /// Retrieves all movies
        /// </summary>
        /// <returns>List of movies</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMovies()
        {
            try
            {
                var movies = await _movieService.GetMoviesAsync();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving movies", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific movie by ID
        /// </summary>
        /// <param name="movieId">Movie identifier</param>
        /// <returns>Movie details</returns>
        [HttpGet("{movieId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMovieById(Guid movieId)
        {
            if (movieId == Guid.Empty)
                return BadRequest(new { message = "Invalid movie ID" });

            try
            {
                var movie = await _movieService.GetMovieByIdAsync(movieId);
                return Ok(movie);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the movie", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves movie poster image
        /// </summary>
        /// <param name="movieId">Movie identifier</param>
        /// <returns>Movie poster image file</returns>
        [HttpGet("{movieId}/poster")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMoviePoster(Guid movieId)
        {
            if (movieId == Guid.Empty)
                return BadRequest(new { message = "Invalid movie ID" });

            try
            {
                var (imageData, contentType) = await _movieService.GetMoviePosterAsync(movieId);

                if (imageData == null || imageData.Length == 0)
                    return NotFound(new { message = "Poster image not found" });

                return File(imageData, contentType ?? "image/jpeg");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the poster", error = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new movie
        /// </summary>
        /// <param name="addMovieDto">Movie creation data</param>
        /// <param name="posterImage">Optional poster image file</param>
        /// <returns>Success message</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddMovie([FromForm] AddMovieDto addMovieDto, [FromForm] IFormFile? posterImage)
        {
            try
            {
                await _movieService.AddMovieAsync(addMovieDto, posterImage);
                return Ok(new { message = "Movie added successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the movie", error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing movie
        /// </summary>
        /// <param name="movieId">Movie identifier</param>
        /// <param name="updateMovieDto">Movie update data</param>
        /// <param name="posterImage">Optional new poster image file</param>
        /// <returns>Success message</returns>
        [HttpPut("{movieId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMovie(Guid movieId, [FromForm] UpdateMovieDto updateMovieDto, [FromForm] IFormFile? posterImage)
        {
            if (movieId == Guid.Empty)
                return BadRequest(new { message = "Invalid movie ID" });

            try
            {
                await _movieService.UpdateMovieAsync(movieId, updateMovieDto, posterImage);
                return Ok(new { message = "Movie updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the movie", error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a movie
        /// </summary>
        /// <param name="movieId">Movie identifier</param>
        /// <returns>Success message</returns>
        [HttpDelete("{movieId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteMovie(Guid movieId)
        {
            if (movieId == Guid.Empty)
                return BadRequest(new { message = "Invalid movie ID" });

            try
            {
                await _movieService.DeleteMovieAsync(movieId);
                return Ok(new { message = "Movie deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the movie", error = ex.Message });
            }
        }
    }
}