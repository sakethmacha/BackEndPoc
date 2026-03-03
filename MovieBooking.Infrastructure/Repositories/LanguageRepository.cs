using Microsoft.EntityFrameworkCore;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Domain.Entities;
using MovieBooking.Infrastructure.Persistence;

namespace MovieBooking.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for language data access operations
    /// </summary>
    public class LanguageRepository : ILanguageRepository
    {
        private readonly MovieBookingDatabaseContext DbContext;

        /// <summary>Initializes a new instance of LanguageRepository</summary>
        public LanguageRepository(MovieBookingDatabaseContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<List<Language>> GetLanguagesAsync()
            => await DbContext.Languages.OrderBy(l => l.Name).ToListAsync();

        /// <inheritdoc/>
        public async Task<Language> GetLanguageByIdAsync(Guid languageId)
        {
            var language = await DbContext.Languages.FindAsync(languageId);
            if (language == null)
                throw new InvalidOperationException("Language not found");
            return language;
        }

        /// <inheritdoc/>
        public async Task<bool> LanguageExistsAsync(string name)
            => await DbContext.Languages.AnyAsync(l => l.Name.ToLower() == name.ToLower());

        /// <inheritdoc/>
        public async Task AddLanguageAsync(Language language)
        {
            DbContext.Languages.Add(language);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateLanguageAsync(Language language)
        {
            DbContext.Languages.Update(language);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteLanguageAsync(Language language)
        {
            DbContext.Languages.Remove(language);
            await DbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> LanguageHasActiveShowTimesAsync(Guid languageId)
            => await DbContext.ShowTimes.AnyAsync(st => st.LanguageId == languageId && st.IsActive);
    }
}