using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Entities;

namespace MovieBooking.Application.Services
{
    /// <summary>
    /// Service implementation for language management operations
    /// </summary>
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository LanguageRepository;

        /// <summary>Initializes a new instance of LanguageService</summary>
        public LanguageService(ILanguageRepository languageRepository)
        {
            LanguageRepository = languageRepository;
        }

        /// <inheritdoc/>
        public async Task<List<LanguageDto>> GetLanguagesAsync()
        {
            var languages = await LanguageRepository.GetLanguagesAsync();
            return languages.Select(l => new LanguageDto
            {
                LanguageId = l.LanguageId,
                Name = l.Name
            }).ToList();
        }

        /// <inheritdoc/>
        public async Task<LanguageDto> GetLanguageByIdAsync(Guid languageId)
        {
            var language = await LanguageRepository.GetLanguageByIdAsync(languageId);
            return new LanguageDto { LanguageId = language.LanguageId, Name = language.Name };
        }

        /// <inheritdoc/>
        public async Task AddLanguageAsync(CreateLanguageDto createLanguageDto)
        {
            var exists = await LanguageRepository.LanguageExistsAsync(createLanguageDto.Name);
            if (exists)
                throw new InvalidOperationException("Language already exists");

            var language = new Language
            {
                LanguageId = Guid.NewGuid(),
                Name = createLanguageDto.Name.Trim()
            };

            await LanguageRepository.AddLanguageAsync(language);
        }

        /// <inheritdoc/>
        public async Task UpdateLanguageAsync(Guid languageId, UpdateLanguageDto updateLanguageDto)
        {
            var language = await LanguageRepository.GetLanguageByIdAsync(languageId);
            var exists = await LanguageRepository.LanguageExistsAsync(updateLanguageDto.Name);
            if (exists && !language.Name.Equals(updateLanguageDto.Name, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Language name already exists");

            language.Name = updateLanguageDto.Name.Trim();
            await LanguageRepository.UpdateLanguageAsync(language);
        }

        /// <inheritdoc/>
        public async Task DeleteLanguageAsync(Guid languageId)
        {
            var language = await LanguageRepository.GetLanguageByIdAsync(languageId);
            var hasActiveShowTimes = await LanguageRepository.LanguageHasActiveShowTimesAsync(languageId);
            if (hasActiveShowTimes)
                throw new InvalidOperationException("Cannot delete language with active showtimes. Please deactivate or delete showtimes first.");
            await LanguageRepository.DeleteLanguageAsync(language);
        }
    }
}