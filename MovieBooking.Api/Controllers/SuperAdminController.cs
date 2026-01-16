using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Enums;
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
        public async Task<IActionResult> AddScreen(CreateScreenRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            await _superAdminService.AddScreenAsync(request);

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
        public async Task<IActionResult> AddShowTime(CreateShowTimeDto request)
        {
            await _superAdminService.AddShowTimeAsync(request);

            return Ok("ShowTimes created successfully");
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
        // ---------- MOVIE UPDATE ----------
        [HttpPut("movies/{movieId}")]
        public async Task<IActionResult> UpdateMovie(Guid movieId, [FromBody] UpdateMovieDto dto)
        {
            if (movieId == Guid.Empty)
                return BadRequest("Invalid movie ID");

            await _superAdminService.UpdateMovieAsync(movieId, dto);
            return Ok(new { message = "Movie updated successfully" });
        }

        // ---------- THEATRE UPDATE ----------
        [HttpPut("theatres/{theatreId}")]
        public async Task<IActionResult> UpdateTheatre(Guid theatreId, [FromBody] UpdateTheatreDto dto)
        {
            if (theatreId == Guid.Empty)
                return BadRequest("Invalid theatre ID");

            await _superAdminService.UpdateTheatreAsync(theatreId, dto);
            return Ok(new { message = "Theatre updated successfully" });
        }

        // ---------- SCREEN UPDATE ----------
        [HttpPut("screens/{screenId}")]
        public async Task<IActionResult> UpdateScreen(Guid screenId, [FromBody] UpdateScreenDto dto)
        {
            if (screenId == Guid.Empty)
                return BadRequest("Invalid screen ID");

            await _superAdminService.UpdateScreenAsync(screenId, dto);
            return Ok(new { message = "Screen updated successfully" });
        }

        // ---------- SHOWTIME UPDATE ----------
        [HttpPut("showtimes/{showTimeId}")]
        public async Task<IActionResult> UpdateShowTime(Guid showTimeId, [FromBody] UpdateShowTimeDto dto)
        {
            if (showTimeId == Guid.Empty)
                return BadRequest("Invalid showtime ID");

            await _superAdminService.UpdateShowTimeAsync(showTimeId, dto);
            return Ok(new { message = "ShowTime updated successfully" });
        }

        // ---------- LANGUAGE UPDATE ----------
        [HttpPut("languages/{languageId}")]
        public async Task<IActionResult> UpdateLanguage(Guid languageId, [FromBody] UpdateLanguageDto dto)
        {
            if (languageId == Guid.Empty)
                return BadRequest("Invalid language ID");

            await _superAdminService.UpdateLanguageAsync(languageId, dto);
            return Ok(new { message = "Language updated successfully" });
        }

        // ---------- ADMIN UPDATE ----------
        [HttpPut("admins/{adminId}")]
        public async Task<IActionResult> UpdateAdmin(Guid adminId, [FromBody] UpdateAdminDto dto)
        {
            if (adminId == Guid.Empty)
                return BadRequest("Invalid admin ID");

            await _superAdminService.UpdateAdminAsync(adminId, dto);
            return Ok(new { message = "Admin updated successfully" });
        }

        // ========== DELETE ENDPOINTS ==========

        // ---------- MOVIE DELETE ----------
        [HttpDelete("movies/{movieId}")]
        public async Task<IActionResult> DeleteMovie(Guid movieId)
        {
            if (movieId == Guid.Empty)
                return BadRequest("Invalid movie ID");

            await _superAdminService.DeleteMovieAsync(movieId);
            return Ok(new { message = "Movie deleted successfully" });
        }

        // ---------- THEATRE DELETE ----------
        [HttpDelete("theatres/{theatreId}")]
        public async Task<IActionResult> DeleteTheatre(Guid theatreId)
        {
            if (theatreId == Guid.Empty)
                return BadRequest("Invalid theatre ID");

            await _superAdminService.DeleteTheatreAsync(theatreId);
            return Ok(new { message = "Theatre deleted successfully" });
        }

        // ---------- SCREEN DELETE ----------
        [HttpDelete("screens/{screenId}")]
        public async Task<IActionResult> DeleteScreen(Guid screenId)
        {
            if (screenId == Guid.Empty)
                return BadRequest("Invalid screen ID");

            await _superAdminService.DeleteScreenAsync(screenId);
            return Ok(new { message = "Screen deleted successfully" });
        }

        // ---------- SHOWTIME DELETE ----------
        [HttpDelete("showtimes/{showTimeId}")]
        public async Task<IActionResult> DeleteShowTime(Guid showTimeId)
        {
            if (showTimeId == Guid.Empty)
                return BadRequest("Invalid showtime ID");

            await _superAdminService.DeleteShowTimeAsync(showTimeId);
            return Ok(new { message = "ShowTime deleted successfully" });
        }

        // ---------- LANGUAGE DELETE ----------
        [HttpDelete("languages/{languageId}")]
        public async Task<IActionResult> DeleteLanguage(Guid languageId)
        {
            if (languageId == Guid.Empty)
                return BadRequest("Invalid language ID");

            await _superAdminService.DeleteLanguageAsync(languageId);
            return Ok(new { message = "Language deleted successfully" });
        }

        // ---------- ADMIN DELETE ----------
        [HttpDelete("admins/{adminId}")]
        public async Task<IActionResult> DeleteAdmin(Guid adminId)
        {
            if (adminId == Guid.Empty)
                return BadRequest("Invalid admin ID");

            await _superAdminService.DeleteAdminAsync(adminId);
            return Ok(new { message = "Admin deleted successfully" });
        }

        // ⭐ These endpoints are REQUIRED for update forms

        [HttpGet("movies/{movieId}")]
        public async Task<IActionResult> GetMovieById(Guid movieId)
        {
            var movie = await _superAdminService.GetMovieByIdAsync(movieId);
            return Ok(movie);
        }

        [HttpGet("theatres/{theatreId}")]
        public async Task<IActionResult> GetTheatreById(Guid theatreId)
        {
            var theatre = await _superAdminService.GetTheatreByIdAsync(theatreId);
            return Ok(theatre);
        }

        [HttpGet("screens/{screenId}")]
        public async Task<IActionResult> GetScreenById(Guid screenId)
        {
            var screen = await _superAdminService.GetScreenByIdAsync(screenId);
            return Ok(screen);
        }

        [HttpGet("showtimes/{showTimeId}")]
        public async Task<IActionResult> GetShowTimeById(Guid showTimeId)
        {
            var showTime = await _superAdminService.GetShowTimeByIdAsync(showTimeId);
            return Ok(showTime);
        }

        [HttpGet("languages/{languageId}")]
        public async Task<IActionResult> GetLanguageById(Guid languageId)
        {
            var language = await _superAdminService.GetLanguageByIdAsync(languageId);
            return Ok(language);
        }

        [HttpGet("admins/{adminId}")]
        public async Task<IActionResult> GetAdminById(Guid adminId)
        {
            var admin = await _superAdminService.GetAdminByIdAsync(adminId);
            return Ok(admin);
        }
    }
}
