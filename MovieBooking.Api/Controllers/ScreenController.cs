using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Services;

namespace MovieBooking.Api.Controllers
{
    /// <summary>
    /// Controller for managing screen operations
    /// </summary>
    [ApiController]
    [Route("api/superadmin/screens")]
    [Authorize(Roles = "SuperAdmin")]
    public class ScreenController : ControllerBase
    {
        private readonly IScreenService _screenService;

        /// <summary>
        /// Initializes a new instance of the ScreenController
        /// </summary>
        /// <param name="screenService">Screen service instance</param>
        public ScreenController(IScreenService screenService)
        {
            _screenService = screenService;
        }

        /// <summary>
        /// Retrieves all screens
        /// </summary>
        /// <returns>List of screens</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetScreens()
        {
            try
            {
                var screens = await _screenService.GetScreensAsync();
                return Ok(screens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving screens", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific screen by ID
        /// </summary>
        /// <param name="screenId">Screen identifier</param>
        /// <returns>Screen details</returns>
        [HttpGet("{screenId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetScreenById(Guid screenId)
        {
            if (screenId == Guid.Empty)
                return BadRequest(new { message = "Invalid screen ID" });

            try
            {
                var screen = await _screenService.GetScreenByIdAsync(screenId);
                return Ok(screen);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the screen", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all screens for a specific theatre
        /// </summary>
        /// <param name="theatreId">Theatre identifier</param>
        /// <returns>List of screens for the theatre</returns>
        [HttpGet("by-theatre/{theatreId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetScreensByTheatre(Guid theatreId)
        {
            if (theatreId == Guid.Empty)
                return BadRequest(new { message = "Invalid theatre ID" });

            try
            {
                var screens = await _screenService.GetScreensByTheatreAsync(theatreId);
                return Ok(screens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving screens", error = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new screen
        /// </summary>
        /// <param name="createScreenRequest">Screen creation data</param>
        /// <returns>Success message</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddScreen(CreateScreenRequest createScreenRequest)
        {
            if (createScreenRequest == null)
                return BadRequest(new { message = "Invalid request" });

            try
            {
                await _screenService.AddScreenAsync(createScreenRequest);
                return Ok(new { message = "Screen added successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the screen", error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing screen
        /// </summary>
        /// <param name="screenId">Screen identifier</param>
        /// <param name="updateScreenDto">Screen update data</param>
        /// <returns>Success message</returns>
        [HttpPut("{screenId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateScreen(Guid screenId, [FromBody] UpdateScreenDto updateScreenDto)
        {
            if (screenId == Guid.Empty)
                return BadRequest(new { message = "Invalid screen ID" });

            try
            {
                await _screenService.UpdateScreenAsync(screenId, updateScreenDto);
                return Ok(new { message = "Screen updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the screen", error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a screen
        /// </summary>
        /// <param name="screenId">Screen identifier</param>
        /// <returns>Success message</returns>
        [HttpDelete("{screenId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteScreen(Guid screenId)
        {
            if (screenId == Guid.Empty)
                return BadRequest(new { message = "Invalid screen ID" });

            try
            {
                await _screenService.DeleteScreenAsync(screenId);
                return Ok(new { message = "Screen deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the screen", error = ex.Message });
            }
        }
    }
}