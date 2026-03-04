using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.DTOs.Admin;
using MovieBooking.Application.Interfaces.Services;
using System.Security.Claims;

namespace MovieBooking.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService RequestService;

        public RequestController(IRequestService requestService)
        {
            RequestService = requestService;
        }

        private Guid GetAdminId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID not found");

            return Guid.Parse(userId);
        }

        // ========== THEATRE REQUESTS ==========

        [HttpPost("theatres/request")]
        public async Task<IActionResult> RequestTheatre(CreateTheatreRequestDto createTheatreRequestDto)
        {
            var adminId = GetAdminId();
            var theatreId = await RequestService.RequestTheatreAsync(createTheatreRequestDto, adminId);
            return Ok(new { message = "Theatre request submitted successfully", theatreId });
        }

        [HttpGet("theatres/requests")]
        public async Task<IActionResult> GetTheatreRequests()
        {
            var adminId = GetAdminId();
            var requests = await RequestService.GetTheatreRequestsAsync(adminId);
            return Ok(requests);
        }

        [HttpGet("theatres/approved")]
        public async Task<IActionResult> GetApprovedTheatres()
        {
            var adminId = GetAdminId();
            var theatres = await RequestService.GetApprovedTheatresAsync(adminId);
            return Ok(theatres);
        }

        // ========== SCREEN REQUESTS ==========

        [HttpPost("screens/request")]
        public async Task<IActionResult> RequestScreen(CreateScreenRequestDto createScreenRequestDto)
        {
            var adminId = GetAdminId();
            var screenId = await RequestService.RequestScreenAsync(createScreenRequestDto, adminId);
            return Ok(new { message = "Screen request submitted successfully", screenId });
        }

        [HttpGet("screens/requests")]
        public async Task<IActionResult> GetScreenRequests()
        {
            var adminId = GetAdminId();
            var requests = await RequestService.GetScreenRequestsAsync(adminId);
            return Ok(requests);
        }

        [HttpGet("screens/approved")]
        public async Task<IActionResult> GetApprovedScreens()
        {
            var adminId = GetAdminId();
            var screens = await RequestService.GetApprovedScreensAsync(adminId);
            return Ok(screens);
        }

        [HttpGet("theatres/for-screen")]
        public async Task<IActionResult> GetTheatresForScreen()
        {
            var adminId = GetAdminId();
            var theatres = await RequestService.GetTheatresForScreenAsync(adminId);
            return Ok(theatres);
        }
    }
}