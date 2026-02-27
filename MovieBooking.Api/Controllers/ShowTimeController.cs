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
    [Route("api/superadmin/showtimes")]
    [Authorize(Roles = "SuperAdmin")]
    public class ShowTimeController : ControllerBase
    {
        private readonly IShowTimeService _showTimeService;

        /// <summary>
        /// Initializes a new instance of the ShowTimeController
        /// </summary>
        /// <param name="showTimeService">ShowTime service instance</param>
        public ShowTimeController(IShowTimeService showTimeService)
        {
            _showTimeService = showTimeService;
        }

        /// <summary>
        /// Retrieves all showtimes
        /// </summary>
        /// <returns>List of showtimes</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShowTimes()
        {
            try
            {
                var showTimes = await _showTimeService.GetShowTimesAsync();
                return Ok(showTimes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving showtimes", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific showtime by ID
        /// </summary>
        /// <param name="showTimeId">ShowTime identifier</param>
        /// <returns>ShowTime details</returns>
        [HttpGet("{showTimeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShowTimeById(Guid showTimeId)
        {
            if (showTimeId == Guid.Empty)
                return BadRequest(new { message = "Invalid showtime ID" });

            try
            {
                var showTime = await _showTimeService.GetShowTimeByIdAsync(showTimeId);
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

        /// <summary>
        /// Creates a new showtime
        /// </summary>
        /// <param name="createShowTimeDto">ShowTime creation data</param>
        /// <returns>Success message</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddShowTime(CreateShowTimeDto createShowTimeDto)
        {
            try
            {
                await _showTimeService.AddShowTimeAsync(createShowTimeDto);
                return Ok(new { message = "ShowTime created successfully" });
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

        /// <summary>
        /// Updates an existing showtime
        /// </summary>
        /// <param name="showTimeId">ShowTime identifier</param>
        /// <param name="updateShowTimeDto">ShowTime update data</param>
        /// <returns>Success message</returns>
        [HttpPut("{showTimeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShowTime(Guid showTimeId, [FromBody] UpdateShowTimeDto updateShowTimeDto)
        {
            if (showTimeId == Guid.Empty)
                return BadRequest(new { message = "Invalid showtime ID" });

            try
            {
                await _showTimeService.UpdateShowTimeAsync(showTimeId, updateShowTimeDto);
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

        /// <summary>
        /// Deletes a showtime
        /// </summary>
        /// <param name="showTimeId">ShowTime identifier</param>
        /// <returns>Success message</returns>
        [HttpDelete("{showTimeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteShowTime(Guid showTimeId)
        {
            if (showTimeId == Guid.Empty)
                return BadRequest(new { message = "Invalid showtime ID" });

            try
            {
                await _showTimeService.DeleteShowTimeAsync(showTimeId);
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