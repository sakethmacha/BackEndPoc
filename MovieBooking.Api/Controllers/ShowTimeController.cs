using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Constants;

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
                return StatusCode(500, new
                {
                    message = MessageStrings.ErrorRetrievingShowTimes,
                    error = ex.Message
                });
            }
        }

        /// <summary>Retrieves a showtime by ID</summary>
        [HttpGet("{showTimeId}")]
        public async Task<IActionResult> GetShowTimeById(Guid showTimeId)
        {
            if (showTimeId == Guid.Empty)
                return BadRequest(new { message = MessageStrings.InvalidShowTimeId });

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
                return StatusCode(500, new
                {
                    message = MessageStrings.ErrorRetrievingShowTime,
                    error = ex.Message
                });
            }
        }

        /// <summary>Adds new showtimes for all theatre time slots</summary>
        [HttpPost]
        public async Task<IActionResult> AddShowTime(CreateShowTimeDto createShowRequest)
        {
            try
            {
                await ShowTimeService.AddShowTimeAsync(createShowRequest);

                return Ok(new
                {
                    message = MessageStrings.ShowTimesCreatedSuccessfully
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
                    message = MessageStrings.ErrorAddingShowTime,
                    error = ex.Message
                });
            }
        }

        /// <summary>Updates an existing showtime</summary>
        [HttpPut("{showTimeId}")]
        public async Task<IActionResult> UpdateShowTime(Guid showTimeId, [FromBody] UpdateShowTimeDto updateShowTimeDto)
        {
            if (showTimeId == Guid.Empty)
                return BadRequest(new { message = MessageStrings.InvalidShowTimeId });

            try
            {
                await ShowTimeService.UpdateShowTimeAsync(showTimeId, updateShowTimeDto);

                return Ok(new
                {
                    message = MessageStrings.ShowTimeUpdatedSuccessfully
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
                    message = MessageStrings.ErrorUpdatingShowTime,
                    error = ex.Message
                });
            }
        }

        /// <summary>Deletes a showtime</summary>
        [HttpDelete("{showTimeId}")]
        public async Task<IActionResult> DeleteShowTime(Guid showTimeId)
        {
            if (showTimeId == Guid.Empty)
                return BadRequest(new { message = MessageStrings.InvalidShowTimeId });

            try
            {
                await ShowTimeService.DeleteShowTimeAsync(showTimeId);

                return Ok(new
                {
                    message = MessageStrings.ShowTimeDeletedSuccessfully
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
                    message = MessageStrings.ErrorDeletingShowTime,
                    error = ex.Message
                });
            }
        }
    }
}