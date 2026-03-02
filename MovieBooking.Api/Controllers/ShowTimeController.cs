using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Services;

namespace MovieBooking.Api.Controllers
{
    /// <summary>
    /// Controller for managing showtime operations
    /// </summary>
    [ApiController]
    [Route("api/showtime")]
    [Authorize(Roles = "SuperAdmin")]
    public class ShowTimeController : ControllerBase
    {
        private readonly IShowTimeService ShowTimeService;

        /// <summary>Initializes a new instance of ShowTimeController</summary>
        public ShowTimeController(IShowTimeService showTimeService)
        {
            ShowTimeService = showTimeService;
        }

        /// <summary>Retrieves all active showtimes</summary>
        [HttpGet]
        public async Task<IActionResult> GetShowTimes()
        {
            try
            {
                var showTimes = await ShowTimeService.GetShowTimesAsync();
                return Ok(showTimes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving showtimes", error = ex.Message });
            }
        }

        /// <summary>Retrieves a showtime by ID</summary>
        [HttpGet("{showTimeId}")]
        public async Task<IActionResult> GetShowTimeById(Guid showTimeId)
        {
            if (showTimeId == Guid.Empty)
                return BadRequest(new { message = "Invalid showtime ID" });
            try
            {
                var showTime = await ShowTimeService.GetShowTimeByIdAsync(showTimeId);
                return Ok(showTime);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the showtime", error = ex.Message });
            }
        }

        /// <summary>Adds new showtimes for all theatre time slots</summary>
        [HttpPost]
        public async Task<IActionResult> AddShowTime(CreateShowTimeDto createShowRequest)
        {
            try
            {
                await ShowTimeService.AddShowTimeAsync(createShowRequest);
                return Ok(new { message = "ShowTimes created successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the showtime", error = ex.Message });
            }
        }

        /// <summary>Updates an existing showtime</summary>
        [HttpPut("{showTimeId}")]
        public async Task<IActionResult> UpdateShowTime(Guid showTimeId, [FromBody] UpdateShowTimeDto updateShowTimeDto)
        {
            if (showTimeId == Guid.Empty)
                return BadRequest(new { message = "Invalid showtime ID" });
            try
            {
                await ShowTimeService.UpdateShowTimeAsync(showTimeId, updateShowTimeDto);
                return Ok(new { message = "ShowTime updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the showtime", error = ex.Message });
            }
        }

        /// <summary>Deletes a showtime</summary>
        [HttpDelete("{showTimeId}")]
        public async Task<IActionResult> DeleteShowTime(Guid showTimeId)
        {
            if (showTimeId == Guid.Empty)
                return BadRequest(new { message = "Invalid showtime ID" });
            try
            {
                await ShowTimeService.DeleteShowTimeAsync(showTimeId);
                return Ok(new { message = "ShowTime deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the showtime", error = ex.Message });
            }
        }
    }
}