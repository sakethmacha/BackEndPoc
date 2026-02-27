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
    [Route("api/superadmin/languages")]
    [Authorize(Roles = "SuperAdmin")]
    public class LanguageController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        /// <summary>
        /// Initializes a new instance of the LanguageController
        /// </summary>
        /// <param name="languageService">Language service instance</param>
        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        /// <summary>
        /// Retrieves all languages
        /// </summary>
        /// <returns>List of languages</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLanguages()
        {
            try
            {
                var languages = await _languageService.GetLanguagesAsync();
                return Ok(languages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving languages", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific language by ID
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language details</returns>
        [HttpGet("{languageId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLanguageById(Guid languageId)
        {
            if (languageId == Guid.Empty)
                return BadRequest(new { message = "Invalid language ID" });

            try
            {
                var language = await _languageService.GetLanguageByIdAsync(languageId);
                return Ok(language);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the language"