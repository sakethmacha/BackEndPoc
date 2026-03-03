using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Services;

namespace MovieBooking.Api.Controllers
{
    /// <summary>
    /// Controller for managing language operations
    /// </summary>
    [ApiController]
    [Route("api/language")]
    [Authorize(Roles = "SuperAdmin")]
    public class LanguageController : ControllerBase
    {
        private readonly ILanguageService LanguageService;

        /// <summary>Initializes a new instance of LanguageController</summary>
        public LanguageController(ILanguageService languageService)
        {
            LanguageService = languageService;
        }

        /// <summary>Retrieves all languages</summary>
        [HttpGet]
        public async Task<IActionResult> GetLanguages()
        {
            try
            {
                var languages = await LanguageService.GetLanguagesAsync();
                return Ok(languages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving languages", error = ex.Message });
            }
        }

        /// <summary>Retrieves a language by ID</summary>
        [HttpGet("{languageId}")]
        public async Task<IActionResult> GetLanguageById(Guid languageId)
        {
            if (languageId == Guid.Empty)
                return BadRequest(new { message = "Invalid language ID" });
            try
            {
                var language = await LanguageService.GetLanguageByIdAsync(languageId);
                return Ok(language);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the language", error = ex.Message });
            }
        }

        /// <summary>Adds a new language</summary>
        [HttpPost]
        public async Task<IActionResult> AddLanguage(CreateLanguageDto createLanguageDto)
        {
            try
            {
                await LanguageService.AddLanguageAsync(createLanguageDto);
                return Ok(new { message = "Language added successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the language", error = ex.Message });
            }
        }

        /// <summary>Updates an existing language</summary>
        [HttpPut("{languageId}")]
        public async Task<IActionResult> UpdateLanguage(Guid languageId, [FromBody] UpdateLanguageDto updateLanguageDto)
        {
            if (languageId == Guid.Empty)
                return BadRequest(new { message = "Invalid language ID" });
            try
            {
                await LanguageService.UpdateLanguageAsync(languageId, updateLanguageDto);
                return Ok(new { message = "Language updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the language", error = ex.Message });
            }
        }

        /// <summary>Deletes a language</summary>
        [HttpDelete("{languageId}")]
        public async Task<IActionResult> DeleteLanguage(Guid languageId)
        {
            if (languageId == Guid.Empty)
                return BadRequest(new { message = "Invalid language ID" });
            try
            {
                await LanguageService.DeleteLanguageAsync(languageId);
                return Ok(new { message = "Language deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the language", error = ex.Message });
            }
        }
    }
}