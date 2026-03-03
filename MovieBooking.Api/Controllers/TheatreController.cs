using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Constants;
using System.Security.Claims;

namespace MovieBooking.Api.Controllers
{
    /// <summary>
    /// Controller for managing theatre operations
    /// </summary>
    [ApiController]
    [Route("api/theatre")]
    [Authorize(Roles = "SuperAdmin")]
    public class TheatreController : ControllerBase
    {
        private readonly ITheatreService TheatreService;

        /// <summary>Initializes a new instance of TheatreController</summary>
        public TheatreController(ITheatreService theatreService)
        {
            TheatreService = theatreService;
        }

        /// <summary>Retrieves all active theatres</summary>
        [HttpGet]
        public async Task<IActionResult> GetTheatres()
        {
            try
            {
                var theatres = await TheatreService.GetTheatresAsync();
                return Ok(theatres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = MessageStrings.ErrorRetrievingTheatres,
                    error = ex.Message
                });
            }
        }

        /// <summary>Retrieves a theatre by ID with time slots</summary>
        [HttpGet("{theatreId}")]
        public async Task<IActionResult> GetTheatreById(Guid theatreId)
        {
            if (theatreId == Guid.Empty)
                return BadRequest(new { message = MessageStrings.InvalidTheatreId });

            try
            {
                var theatre = await TheatreService.GetTheatreByIdAsync(theatreId);
                return Ok(theatre);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = MessageStrings.ErrorRetrievingTheatre,
                    error = ex.Message
                });
            }
        }

        /// <summary>Adds a new theatre with time slots</summary>
        [HttpPost]
        public async Task<IActionResult> AddTheatre(CreateTheatreDto createTheatreDto)
        {
            try
            {
                var superAdminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                await TheatreService.AddTheatreAsync(createTheatreDto, superAdminId);

                return Ok(new
                {
                    message = MessageStrings.TheatreAddedSuccessfully
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
                    message = MessageStrings.ErrorAddingTheatre,
                    error = ex.Message
                });
            }
        }

        /// <summary>Updates an existing theatre</summary>
        [HttpPut("{theatreId}")]
        public async Task<IActionResult> UpdateTheatre(Guid theatreId, [FromBody] UpdateTheatreDto updateTheatreDto)
        {
            if (theatreId == Guid.Empty)
                return BadRequest(new { message = MessageStrings.InvalidTheatreId });

            try
            {
                await TheatreService.UpdateTheatreAsync(theatreId, updateTheatreDto);

                return Ok(new
                {
                    message = MessageStrings.TheatreUpdatedSuccessfully
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
                    message = MessageStrings.ErrorUpdatingTheatre,
                    error = ex.Message
                });
            }
        }

        /// <summary>Deletes a theatre</summary>
        [HttpDelete("{theatreId}")]
        public async Task<IActionResult> DeleteTheatre(Guid theatreId)
        {
            if (theatreId == Guid.Empty)
                return BadRequest(new { message = MessageStrings.InvalidTheatreId });

            try
            {
                await TheatreService.DeleteTheatreAsync(theatreId);

                return Ok(new
                {
                    message = MessageStrings.TheatreDeletedSuccessfully
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
                    message = MessageStrings.ErrorDeletingTheatre,
                    error = ex.Message
                });
            }
        }
    }
}