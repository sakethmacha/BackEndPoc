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
    [Route("api/screen")]
    [Authorize(Roles = "SuperAdmin")]
    public class ScreenController : ControllerBase
    {
        private readonly IScreenService ScreenService;

        /// <summary>Initializes a new instance of ScreenController</summary>
        public ScreenController(IScreenService screenService)
        {
            ScreenService = screenService;
        }

        /// <summary>Retrieves all active screens</summary>
        [HttpGet]
        public async Task<IActionResult> GetScreens()
        {
            try
            {
                var screens = await ScreenService.GetScreensAsync();
                return Ok(screens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving screens", error = ex.Message });
            }
        }

        /// <summary>Retrieves a screen by ID with seat layout</summary>
        [HttpGet("{screenId}")]
        public async Task<IActionResult> GetScreenById(Guid screenId)
        {
            if (screenId == Guid.Empty)
                return BadRequest(new { message = "Invalid screen ID" });
            try
            {
                var screen = await ScreenService.GetScreenByIdAsync(screenId);
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

        /// <summary>Retrieves screens by theatre ID</summary>
        [HttpGet("by-theatre/{theatreId}")]
        public async Task<IActionResult> GetScreensByTheatre(Guid theatreId)
        {
            if (theatreId == Guid.Empty)
                return BadRequest(new { message = "Invalid theatre ID" });
            try
            {
                var screens = await ScreenService.GetScreensByTheatreAsync(theatreId);
                return Ok(screens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving screens", error = ex.Message });
            }
        }

        /// <summary>Adds a new screen with seats</summary>
        [HttpPost]
        public async Task<IActionResult> AddScreen(CreateScreenRequest createScreenRequest)
        {
            if (createScreenRequest == null)
                return BadRequest(new { message = "Invalid request" });
            try
            {
                await ScreenService.AddScreenAsync(createScreenRequest);
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

        /// <summary>Updates an existing screen</summary>
        [HttpPut("{screenId}")]
        public async Task<IActionResult> UpdateScreen(Guid screenId, [FromBody] UpdateScreenDto updateScreenDto)
        {
            if (screenId == Guid.Empty)
                return BadRequest(new { message = "Invalid screen ID" });
            try
            {
                await ScreenService.UpdateScreenAsync(screenId, updateScreenDto);
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

        /// <summary>Deletes a screen</summary>
        [HttpDelete("{screenId}")]
        public async Task<IActionResult> DeleteScreen(Guid screenId)
        {
            if (screenId == Guid.Empty)
                return BadRequest(new { message = "Invalid screen ID" });
            try
            {
                await ScreenService.DeleteScreenAsync(screenId);
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