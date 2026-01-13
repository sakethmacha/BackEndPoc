using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Services;
using System.Security.Claims;

namespace MovieBooking.Api.Controllers
{
    [ApiController]
    [Route("api/superadmin")]
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : ControllerBase
    {
        private readonly ISuperAdminService _superAdminService;

        public SuperAdminController(ISuperAdminService superAdminService)
        {
            _superAdminService = superAdminService;
        }

        // ---------- ADMIN ----------
        [HttpPost("admins")]
        public async Task<IActionResult> CreateAdmin(CreateAdminDto dto)
        {
            await _superAdminService.CreateAdminAsync(dto);
            return Ok();
        }

        [HttpGet("admins")]
        public async Task<IActionResult> GetAdmins()
            => Ok(await _superAdminService.GetAdminsAsync());

        [HttpPut("admins/{adminId}/toggle")]
        public async Task<IActionResult> ToggleAdmin(Guid adminId)
        {
            await _superAdminService.ToggleAdminAsync(adminId);
            return Ok();
        }

        // ---------- MOVIE ----------
        [HttpPost("movies")]
        public async Task<IActionResult> AddMovie(AddMovieDto dto)
        {
            var isAuth = User.Identity?.IsAuthenticated;
            var name = User.Identity?.Name;
            await _superAdminService.AddMovieAsync(dto);
            return Ok();
        }

        [HttpPut("movies/{movieId}/toggle")]
        public async Task<IActionResult> ToggleMovie(Guid movieId)
        {
            await _superAdminService.ToggleMovieAsync(movieId);
            return Ok();
        }
        [HttpGet("movies")]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _superAdminService.GetMoviesAsync();
            return Ok(movies);
        }

        // ---------- THEATRE ----------
        [HttpPost("theatres")]
        public async Task<IActionResult> AddTheatre(CreateTheatreDto dto)
        {
            //var superAdminId = Guid.Parse(User.FindFirst("UserId")!.Value);
            var superAdminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
           
            await _superAdminService.AddTheatreAsync(dto, superAdminId);
            return Ok("Theatre added successfully");
        }
        [HttpGet("theatres")]
        public async Task<IActionResult> GetTheatres()
        {
            var theatres = await _superAdminService.GetTheatresAsync();
            return Ok(theatres);
        }

        [HttpPost("screens")]
        public async Task<IActionResult> AddScreen(CreateScreenDto dto)
        {
            if (dto == null)
                return BadRequest("Screen data is required");
            await _superAdminService.AddScreenAsync(dto);
            return Ok("Screen added successfully");
        }
        [HttpGet("screens")]
        public async Task<IActionResult> GetScreens()
        {
            var screens = await _superAdminService.GetScreensAsync();
            return Ok(screens);
        }
        // ---------- SHOWTIME ----------
        [HttpPost("showtimes")]
        public async Task<IActionResult> AddShowTime(CreateShowTimeDto dto)
        {
            await _superAdminService.AddShowTimeAsync(dto);
            return Ok("ShowTime added successfully");
        }
        [HttpGet("showtimes")]
        public async Task<IActionResult> GetShowTimes()
        {
            var showTimes = await _superAdminService.GetShowTimesAsync();
            return Ok(showTimes);
        }
        // ---------- APPROVAL ----------
        [HttpPut("requests/{requestId}/approve")]
        public async Task<IActionResult> ApproveRequest(Guid requestId)
        {
            await _superAdminService.ApproveRequestAsync(requestId);
            return Ok();
        }
        [HttpGet("screens/by-theatre/{theatreId}")]
        public async Task<IActionResult> GetScreensByTheatre(Guid theatreId)
        {
            if (theatreId == Guid.Empty)
                return BadRequest("Invalid theatre id");

            var screens = await _superAdminService.GetScreensByTheatreAsync(theatreId);
            return Ok(screens);
        }

        [HttpPut("requests/{requestId}/reject")]
        public async Task<IActionResult> RejectRequest(Guid requestId)
        {
            await _superAdminService.RejectRequestAsync(requestId);
            return Ok();
        }

        [HttpPost("languages")]
        public async Task<IActionResult> AddLanguage(CreateLanguageDto dto)
        {
            await _superAdminService.AddLanguageAsync(dto);
            return Ok("Language added");
        }

        [HttpGet("languages")]
        public async Task<IActionResult> GetLanguages()
        {
            return Ok(await _superAdminService.GetLanguagesAsync());
        }

    }
}
