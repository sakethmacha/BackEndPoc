using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for language management operations
    /// </summary>
    public interface ILanguageService
    {
        /// <summary>Retrieves all languages</summary>
        Task<List<LanguageDto>> GetLanguagesAsync();

        /// <summary>Retrieves a language by ID</summary>
        Task<LanguageDto> GetLanguageByIdAsync(Guid languageId);

        /// <summary>Adds a new language</summary>
        Task AddLanguageAsync(CreateLanguageDto createLanguageDto);

        /// <summary>Updates an existing language</summary>
        Task UpdateLanguageAsync(Guid languageId, UpdateLanguageDto updateLanguageDto);

        /// <summary>Deletes a language</summary>
        Task DeleteLanguageAsync(Guid languageId);
    }
}