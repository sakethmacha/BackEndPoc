using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for language management operations
    /// </summary>
    public interface ILanguageService
    {
        /// <summary>
        /// Retrieves all languages
        /// </summary>
        /// <returns>List of languages</returns>
        Task<List<LanguageDto>> GetLanguagesAsync();

        /// <summary>
        /// Retrieves a specific language by ID
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language details</returns>
        Task<LanguageDto> GetLanguageByIdAsync(Guid languageId);

        /// <summary>
        /// Adds a new language
        /// </summary>
        /// <param name="createLanguageDto">Language data</param>
        Task AddLanguageAsync(CreateLanguageDto createLanguageDto);

        /// <summary>
        /// Updates an existing language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="updateLanguageDto">Updated language data</param>
        Task UpdateLanguageAsync(Guid languageId, UpdateLanguageDto updateLanguageDto);

        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        Task DeleteLanguageAsync(Guid languageId);
    }
}