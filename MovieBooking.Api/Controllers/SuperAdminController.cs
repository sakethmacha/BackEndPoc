
using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Services;
using System.Security.Claims;
namespace MovieBooking.Api.Controllers
{
    [ApiController]
    [Route("api/superadmin")]
   // [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : ControllerBase
    {
        private readonly ISuperAdminService SuperAdminService;

        public SuperAdminController(ISuperAdminService superAdminService)
        {
            SuperAdminService = superAdminService;
        }

        // ---------- ADMIN ----------
        [HttpPost("admins")]
        public async Task<IActionResult> CreateAdmin(CreateAdminDto createAdminDto)
        {
            await SuperAdminService.CreateAdminAsync(createAdminDto);
            return Ok();
        }

        [HttpGet("admins")]
        public async Task<IActionResult> GetAdmins()
            => Ok(await SuperAdminService.GetAdminsAsync());

        [HttpPut("admins/{adminId}/toggle")]
        public async Task<IActionResult> ToggleAdmin(Guid adminId)
        {
            await SuperAdminService.ToggleAdminAsync(adminId);
            return Ok();
        }

        // ---------- MOVIE ----------
        [HttpPost("movies")]
        public async Task<IActionResult> AddMovie(AddMovieDto addMovieDto)
        {
            var isAuth = User.Identity?.IsAuthenticated;
            var name = User.Identity?.Name;
            await SuperAdminService.AddMovieAsync(addMovieDto);
            return Ok();
        }

        [HttpPut("movies/{movieId}/toggle")]
        public async Task<IActionResult> ToggleMovie(Guid movieId)
        {
            await SuperAdminService.ToggleMovieAsync(movieId);
            return Ok();
        }
        [HttpGet("movies")]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await SuperAdminService.GetMoviesAsync();
            return Ok(movies);
        }

        // ---------- THEATRE ----------
        [HttpPost("theatres")]
        public async Task<IActionResult> AddTheatre(CreateTheatreDto createTheatreDto)
        {
            //var superAdminId = Guid.Parse(User.FindFirst("UserId")!.Value);
            var superAdminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
           
            await SuperAdminService.AddTheatreAsync(createTheatreDto, superAdminId);
            return Ok("Theatre added successfully");
        }
        [HttpGet("theatres")]
        public async Task<IActionResult> GetTheatres()
        {
            var theatres = await SuperAdminService.GetTheatresAsync();
            return Ok(theatres);
        }

        [HttpPost("screens")]
        public async Task<IActionResult> AddScreen(CreateScreenRequest CreateScreenRequest)
        {
            if (CreateScreenRequest == null)
                return BadRequest("Invalid request");

            await SuperAdminService.AddScreenAsync(CreateScreenRequest);

            return Ok("Screen added successfully");
        }


        
        [HttpGet("screens")]
        public async Task<IActionResult> GetScreens()
        {
            var screens = await SuperAdminService.GetScreensAsync();
            return Ok(screens);
        }
        // ---------- SHOWTIME ----------
        [HttpPost("showtimes")]
        public async Task<IActionResult> AddShowTime(CreateShowTimeDto CreateShowRequest)
        {
            await SuperAdminService.AddShowTimeAsync(CreateShowRequest);

            return Ok("ShowTimes created successfully");
        }

        [HttpGet("showtimes")]
        public async Task<IActionResult> GetShowTimes()
        {
            var showTimes = await SuperAdminService.GetShowTimesAsync();
            return Ok(showTimes);
        }
        // ---------- APPROVAL ----------
        [HttpPut("requests/{requestId}/approve")]
        public async Task<IActionResult> ApproveRequest(Guid requestId)
        {
            await SuperAdminService.ApproveRequestAsync(requestId);
            return Ok();
        }
       
        [HttpPut("requests/{requestId}/reject")]
        public async Task<IActionResult> RejectRequest(Guid requestId)
        {
            await SuperAdminService.RejectRequestAsync(requestId);
            return Ok();
        }
        [HttpPost("languages")]
        public async Task<IActionResult> AddLanguage(CreateLanguageDto createLanguageDto)
        {
            await SuperAdminService.AddLanguageAsync(createLanguageDto);
            return Ok("Language added");
        }
        [HttpGet("screens/by-theatre/{theatreId}")]
        public async Task<IActionResult> GetScreensByTheatre(Guid theatreId)
        {
            if (theatreId == Guid.Empty)
                return BadRequest("Invalid theatre id");

            var screens = await SuperAdminService.GetScreensByTheatreAsync(theatreId);
            return Ok(screens);
        }

        [HttpGet("languages")]
        public async Task<IActionResult> GetLanguages()
        {
            return Ok(await SuperAdminService.GetLanguagesAsync());
        }
        // ---------- MOVIE UPDATE ----------
        [HttpPut("movies/{movieId}")]
        public async Task<IActionResult> UpdateMovie(Guid movieId, [FromBody] UpdateMovieDto updateMovieDto)
        {
            if (movieId == Guid.Empty)
                return BadRequest("Invalid movie ID");

            await SuperAdminService.UpdateMovieAsync(movieId, updateMovieDto);
            return Ok(new { message = "Movie updated successfully" });
        }

        // ---------- THEATRE UPDATE ----------
        [HttpPut("theatres/{theatreId}")]
        public async Task<IActionResult> UpdateTheatre(Guid theatreId, [FromBody] UpdateTheatreDto updateTheatreDto)
        {
            if (theatreId == Guid.Empty)
                return BadRequest("Invalid theatre ID");

            await SuperAdminService.UpdateTheatreAsync(theatreId, updateTheatreDto);
            return Ok(new { message = "Theatre updated successfully" });
        }

        // ---------- SCREEN UPDATE ----------
        [HttpPut("screens/{screenId}")]
        public async Task<IActionResult> UpdateScreen(Guid screenId, [FromBody] UpdateScreenDto updateScreenDto)
        {
            if (screenId == Guid.Empty)
                return BadRequest("Invalid screen ID");

            await SuperAdminService.UpdateScreenAsync(screenId, updateScreenDto);
            return Ok(new { message = "Screen updated successfully" });
        }

        // ---------- SHOWTIME UPDATE ----------
        [HttpPut("showtimes/{showTimeId}")]
        public async Task<IActionResult> UpdateShowTime(Guid showTimeId, [FromBody] UpdateShowTimeDto updateShowTimeDto)
        {
            if (showTimeId == Guid.Empty)
                return BadRequest("Invalid showtime ID");

            await SuperAdminService.UpdateShowTimeAsync(showTimeId, updateShowTimeDto);
            return Ok(new { message = "ShowTime updated successfully" });
        }

        // ---------- LANGUAGE UPDATE ----------
        [HttpPut("languages/{languageId}")]
        public async Task<IActionResult> UpdateLanguage(Guid languageId, [FromBody] UpdateLanguageDto updateLanguageDto)
        {
            if (languageId == Guid.Empty)
                return BadRequest("Invalid language ID");

            await SuperAdminService.UpdateLanguageAsync(languageId, updateLanguageDto);
            return Ok(new { message = "Language updated successfully" });
        }

        // ---------- ADMIN UPDATE ----------
        [HttpPut("admins/{adminId}")]
        public async Task<IActionResult> UpdateAdmin(Guid adminId, [FromBody] UpdateAdminDto updateAdminDto)
        {
            if (adminId == Guid.Empty)
                return BadRequest("Invalid admin ID");

            await SuperAdminService.UpdateAdminAsync(adminId, updateAdminDto);
            return Ok(new { message = "Admin updated successfully" });
        }

        // ========== DELETE ENDPOINTS ==========

        // ---------- MOVIE DELETE ----------
        [HttpDelete("movies/{movieId}")]
        public async Task<IActionResult> DeleteMovie(Guid movieId)
        {
            if (movieId == Guid.Empty)
                return BadRequest("Invalid movie ID");

            await SuperAdminService.DeleteMovieAsync(movieId);
            return Ok(new { message = "Movie deleted successfully" });
        }

        // ---------- THEATRE DELETE ----------
        [HttpDelete("theatres/{theatreId}")]
        public async Task<IActionResult> DeleteTheatre(Guid theatreId)
        {
            if (theatreId == Guid.Empty)
                return BadRequest("Invalid theatre ID");

            await SuperAdminService.DeleteTheatreAsync(theatreId);
            return Ok(new { message = "Theatre deleted successfully" });
        }

        // ---------- SCREEN DELETE ----------
        [HttpDelete("screens/{screenId}")]
        public async Task<IActionResult> DeleteScreen(Guid screenId)
        {
            if (screenId == Guid.Empty)
                return BadRequest("Invalid screen ID");

            await SuperAdminService.DeleteScreenAsync(screenId);
            return Ok(new { message = "Screen deleted successfully" });
        }

        // ---------- SHOWTIME DELETE ----------
        [HttpDelete("showtimes/{showTimeId}")]
        public async Task<IActionResult> DeleteShowTime(Guid showTimeId)
        {
            if (showTimeId == Guid.Empty)
                return BadRequest("Invalid showtime ID");

            await SuperAdminService.DeleteShowTimeAsync(showTimeId);
            return Ok(new { message = "ShowTime deleted successfully" });
        }

        // ---------- LANGUAGE DELETE ----------
        [HttpDelete("languages/{languageId}")]
        public async Task<IActionResult> DeleteLanguage(Guid languageId)
        {
            if (languageId == Guid.Empty)
                return BadRequest("Invalid language ID");

            await SuperAdminService.DeleteLanguageAsync(languageId);
            return Ok(new { message = "Language deleted successfully" });
        }

        // ---------- ADMIN DELETE ----------
        [HttpDelete("admins/{adminId}")]
        public async Task<IActionResult> DeleteAdmin(Guid adminId)
        {
            if (adminId == Guid.Empty)
                return BadRequest("Invalid admin ID");

            await SuperAdminService.DeleteAdminAsync(adminId);
            return Ok(new { message = "Admin deleted successfully" });
        }

        // ⭐ These endpoints are REQUIRED for update forms

        [HttpGet("movies/{movieId}")]
        public async Task<IActionResult> GetMovieById(Guid movieId)
        {
            var movie = await SuperAdminService.GetMovieByIdAsync(movieId);
            return Ok(movie);
        }

        [HttpGet("theatres/{theatreId}")]
        public async Task<IActionResult> GetTheatreById(Guid theatreId)
        {
            var theatre = await SuperAdminService.GetTheatreByIdAsync(theatreId);
            return Ok(theatre);
        }

        [HttpGet("screens/{screenId}")]
        public async Task<IActionResult> GetScreenById(Guid screenId)
        {
            var screen = await SuperAdminService.GetScreenByIdAsync(screenId);
            return Ok(screen);
        }

        [HttpGet("showtimes/{showTimeId}")]
        public async Task<IActionResult> GetShowTimeById(Guid showTimeId)
        {
            var showTime = await SuperAdminService.GetShowTimeByIdAsync(showTimeId);
            return Ok(showTime);
        }

        [HttpGet("languages/{languageId}")]
        public async Task<IActionResult> GetLanguageById(Guid languageId)
        {
            var language = await SuperAdminService.GetLanguageByIdAsync(languageId);
            return Ok(language);
        }

        [HttpGet("admins/{adminId}")]
        public async Task<IActionResult> GetAdminById(Guid adminId)
        {
            var admin = await SuperAdminService.GetAdminByIdAsync(adminId);
            return Ok(admin);
        }
    }
}
