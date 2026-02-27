using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Services;
using System.Security.Claims;

namespace MovieBooking.Api.Controllers
{
    /// <summary>
    /// Controller for managing theatre operations
    /// </summary>
    [ApiController]
    [Route("api/superadmin/theatres")]
    [Authorize(Roles = "SuperAdmin")]
    public class TheatreController : ControllerBase
    {
        private readonly ITheatreService _theatreService;

        /// <summary>
        /// Initializes a new instance of the TheatreController
        /// </summary>
        /// <param name="theatreService">Theatre service instance</param>
        public TheatreController(ITheatreService theatreService)
        {
            _theatreService = theatreService;
        }

        /// <summary>
        /// Retrieves all theatres
        /// </summary>
        /// <returns>List of theatres</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTheatres()
        {
            try
            {
                var theatres = await _theatreService.GetTheatresAsync();
                return Ok(theatres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving theatres", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific theatre by ID
        /// </summary>
        /// <param name="theatreId">Theatre identifier</param>
        /// <returns>Theatre details</returns>
        [HttpGet("{theatreId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTheatreById(Guid theatreId)
        {
            if (theatreId == Guid.Empty)
                return BadRequest(new { message = "Invalid theatre ID" });

            try
            {
                var theatre = await _theatreService.GetTheatreByIdAsync(theatreId);
                return Ok(theatre);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the theatre", error = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new theatre
        /// </summary>
        /// <param name="createTheatreDto">Theatre creation data</param>
        /// <returns>Success message</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddTheatre(CreateTheatreDto createTheatreDto)
        {
            try
            {
                var superAdminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _theatreService.AddTheatreAsync(createTheatreDto, superAdminId);
                return Ok(new { message = "Theatre added successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the theatre", error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing theatre
        /// </summary>
        /// <param name="theatreId">Theatre identifier</param>
        /// <param name="updateTheatreDto">Theatre update data</param>
        /// <returns>Success message</returns>
        [HttpPut("{theatreId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTheatre(Guid theatreId, [FromBody] UpdateTheatreDto updateTheatreDto)
        {
            if (theatreId == Guid.Empty)
                return BadRequest(new { message = "Invalid theatre ID" });

            try
            {
                await _theatreService.UpdateTheatreAsync(theatreId, updateTheatreDto);
                return Ok(new { message = "Theatre updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the theatre", error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a theatre
        /// </summary>
        /// <param name="theatreId">Theatre identifier</param>
        /// <returns>Success message</returns>
        [HttpDelete("{theatreId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteTheatre(Guid theatreId)
        {
            if (theatreId == Guid.Empty)
                return BadRequest(new { message = "Invalid theatre ID" });

            try
            {
                await _theatreService.DeleteTheatreAsync(theatreId);
                return Ok(new { message = "Theatre deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the theatre", error = ex.Message });
            }
        }
    }
}