using MovieBooking.Domain.Entities;

namespace MovieBooking.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for language data access operations
    /// </summary>
    public interface ILanguageRepository
    {
        /// <summary>Retrieves all languages ordered by name</summary>
        Task<List<Language>> GetLanguagesAsync();

        /// <summary>Retrieves a language by ID</summary>
        Task<Language> GetLanguageByIdAsync(Guid languageId);

        /// <summary>Checks if a language with the same name exists</summary>
        Task<bool> LanguageExistsAsync(string name);

        /// <summary>Adds a new language</summary>
        Task AddLanguageAsync(Language language);

        /// <summary>Updates an existing language</summary>
        Task UpdateLanguageAsync(Language language);

        /// <summary>Deletes a language (hard delete)</summary>
        Task DeleteLanguageAsync(Language language);

        /// <summary>Checks if language has active showtimes</summary>
        Task<bool> LanguageHasActiveShowTimesAsync(Guid languageId);
    }
}