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
        private readonly IRequestService AdminService;

        public RequestController(IRequestService adminService)
        {
            AdminService = adminService;
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
            var theatreId = await AdminService.RequestTheatreAsync(createTheatreRequestDto, adminId);
            return Ok(new { message = "Theatre request submitted successfully", theatreId });
        }

        [HttpGet("theatres/requests")]
        public async Task<IActionResult> GetMyTheatreRequests()
        {
            var adminId = GetAdminId();
            var requests = await AdminService.GetMyTheatreRequestsAsync(adminId);
            return Ok(requests);
        }

        [HttpGet("theatres/approved")]
        public async Task<IActionResult> GetMyApprovedTheatres()
        {
            var adminId = GetAdminId();
            var theatres = await AdminService.GetMyApprovedTheatresAsync(adminId);
            return Ok(theatres);
        }

        // ========== SCREEN REQUESTS ==========

        [HttpPost("screens/request")]
        public async Task<IActionResult> RequestScreen(CreateScreenRequestDto createScreenRequestDto)
        {
            var adminId = GetAdminId();
            var screenId = await AdminService.RequestScreenAsync(createScreenRequestDto, adminId);
            return Ok(new { message = "Screen request submitted successfully", screenId });
        }

        [HttpGet("screens/requests")]
        public async Task<IActionResult> GetMyScreenRequests()
        {
            var adminId = GetAdminId();
            var requests = await AdminService.GetMyScreenRequestsAsync(adminId);
            return Ok(requests);
        }

        [HttpGet("screens/approved")]
        public async Task<IActionResult> GetMyApprovedScreens()
        {
            var adminId = GetAdminId();
            var screens = await AdminService.GetMyApprovedScreensAsync(adminId);
            return Ok(screens);
        }

        [HttpGet("theatres/for-screen")]
        public async Task<IActionResult> GetMyTheatresForScreen()
        {
            var adminId = GetAdminId();
            var theatres = await AdminService.GetMyTheatresForScreenAsync(adminId);
            return Ok(theatres);
        }
    }
}