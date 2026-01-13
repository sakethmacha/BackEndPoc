using Microsoft.EntityFrameworkCore;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;
using MovieBooking.Infrastructure.Persistence;

namespace MovieBooking.Infrastructure.Repositories
{
    public class SuperAdminRepository : ISuperAdminRepository
    {
        private readonly MovieBookingDatabaseContext _db;

        public SuperAdminRepository(MovieBookingDatabaseContext db)
        {
            _db = db;
        }

        public async Task CreateAdminAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public Task<User> GetUserByIdAsync(Guid userId)
            => _db.Users.FindAsync(userId).AsTask();

        public async Task UpdateUserAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task<List<AdminDto>> GetAdminsAsync()
        {
            return await _db.Users
                .Where(u => u.Role == UserRole.Admin)
                .Select(u => new AdminDto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    IsActive = u.IsActive
                }).ToListAsync();
        }

        public async Task AddMovieAsync(Movie movie)
        {
            _db.Movies.Add(movie);
            await _db.SaveChangesAsync();
        }
        public async Task<List<Movie>> GetAllAsync()
        {
            return await _db.Movies
                .AsNoTracking()
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
        }
        public Task<Movie> GetMovieByIdAsync(Guid movieId)
            => _db.Movies.FindAsync(movieId).AsTask();

        public async Task UpdateMovieAsync(Movie movie)
        {
            _db.Movies.Update(movie);
            await _db.SaveChangesAsync();
        }

        public async Task AddTheatreAsync(Theatre theatre)
        {
            _db.Theatres.Add(theatre);
            await _db.SaveChangesAsync();
        }
        public async Task<List<Theatre>> GetTheatresAsync()
       => await _db.Theatres.AsNoTracking().ToListAsync();
        public async Task AddScreenAsync(Screen screen)
        {
            _db.Screens.Add(screen);
            await _db.SaveChangesAsync();
        }
        public async Task<List<Screen>> GetScreensAsync()
        => await _db.Screens.AsNoTracking().ToListAsync();
        public async Task<Language?> GetLanguageByIdAsync(Guid languageId)
        {
            return await _db.Languages.FindAsync(languageId);
        }

        public async Task AddShowTimeAsync(ShowTime showTime)
        {
            _db.ShowTimes.Add(showTime);
            await _db.SaveChangesAsync();
        }
        public async Task<List<ShowTime>> GetShowTimesAsync()
        => await _db.ShowTimes.AsNoTracking().ToListAsync();
        public Task<AdminRequest> GetRequestByIdAsync(Guid requestId)
            => _db.AdminRequests.FindAsync(requestId).AsTask();

        public async Task UpdateRequestAsync(AdminRequest request)
        {
            _db.AdminRequests.Update(request);
            await _db.SaveChangesAsync();
        }

        public async Task ApproveTheatreAsync(Guid theatreId)
        {
            var theatre = await _db.Theatres.FindAsync(theatreId);
            theatre.IsActive = true;
            theatre.ApprovalStatus = ApprovalStatus.APPROVED;
        }
        public async Task<List<Screen>> GetByTheatreIdAsync(Guid theatreId)
        {
            return await _db.Screens
                .Where(s => s.TheatreId == theatreId && s.IsActive)
                .OrderBy(s => s.ScreenName)
                .ToListAsync();
        }
        public async Task ApproveScreenAsync(Guid screenId)
        {
            var screen = await _db.Screens.FindAsync(screenId);
            screen.IsActive = true;
        }

        public async Task ApproveShowTimeAsync(Guid showTimeId)
        {
            var showTime = await _db.ShowTimes.FindAsync(showTimeId);
            showTime.IsActive = true;
            showTime.ApprovalStatus = ApprovalStatus.APPROVED;
        }

        public async Task AddLanguageAsync(Language language)
        {
            _db.Languages.Add(language);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Language>> GetLanguagesAsync()
        {
            return await _db.Languages
                            .OrderBy(l => l.Name)
                            .ToListAsync();
        }
        public async Task<bool> LanguageExistsAsync(string name)
        {
            return await _db.Languages
                .AnyAsync(l => l.Name.ToLower() == name.ToLower());
        }
       
    }
}
