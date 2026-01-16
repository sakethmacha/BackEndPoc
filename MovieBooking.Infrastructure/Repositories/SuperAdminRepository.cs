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
        
        public async Task AddSeatsAsync(List<Seat> seats)
        {
            _db.Seats.AddRange(seats);
            await _db.SaveChangesAsync();
        }

  
        public async Task<List<ShowTime>> GetShowTimesAsync()
        {
            return await _db.ShowTimes
                .Include(st => st.Movie)
                .Include(st => st.Theatre)
                .Include(st => st.Screen)
                .Include(st => st.Language)
                .AsNoTracking()
                .OrderBy(st => st.StartTime)
                .ToListAsync();
        }
        public async Task<List<TheatreTimeSlot>> GetTimeSlotsByTheatreAsync(Guid theatreId)
        {
            return await _db.TheatreTimeSlots
                .Where(t => t.TheatreId == theatreId && t.IsActive)
                .OrderBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task AddShowTimesAsync(List<ShowTime> showTimes)
        {
            _db.ShowTimes.AddRange(showTimes);
            await _db.SaveChangesAsync();
        }

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
        public async Task<bool> ShowTimeConflictExistsAsync(
    Guid screenId, DateTime start, DateTime end)
        {
            return await _db.ShowTimes.AnyAsync(st =>
                st.ScreenId == screenId &&
                st.StartTime < end &&
                start < st.EndTime);
        }

        public async Task ApproveShowTimeAsync(Guid showTimeId)
        {
            var showTime = await _db.ShowTimes.FindAsync(showTimeId);
            showTime.IsActive = true;
            showTime.ApprovalStatus = ApprovalStatus.APPROVED;
        }
        public async Task AddTheatreWithTimeSlotsAsync(
    Theatre theatre,
    List<TheatreTimeSlot> timeSlots)
        {
            _db.Theatres.Add(theatre);
            _db.TheatreTimeSlots.AddRange(timeSlots);
            await _db.SaveChangesAsync();
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
        public async Task<Theatre> GetTheatreByIdAsync(Guid theatreId)
        {
            var theatre = await _db.Theatres
                .Include(t => t.TimeSlots)
                .FirstOrDefaultAsync(t => t.TheatreId == theatreId);

            if (theatre == null)
                throw new InvalidOperationException("Theatre not found");

            return theatre;
        }

        public async Task<Screen> GetScreenByIdAsync(Guid screenId)
        {
            var screen = await _db.Screens
                .Include(s => s.Seats)
                .FirstOrDefaultAsync(s => s.ScreenId == screenId);

            if (screen == null)
                throw new InvalidOperationException("Screen not found");

            return screen;
        }

        public async Task<ShowTime> GetShowTimeByIdAsync(Guid showTimeId)
        {
            var showTime = await _db.ShowTimes.FindAsync(showTimeId);

            if (showTime == null)
                throw new InvalidOperationException("ShowTime not found");

            return showTime;
        }

        public async Task<Language> GetLanguageByIdAsync(Guid languageId)
        {
            var language = await _db.Languages.FindAsync(languageId);

            if (language == null)
                throw new InvalidOperationException("Language not found");

            return language;
        }

        // ========== UPDATE METHODS ==========

        public async Task UpdateTheatreAsync(Theatre theatre)
        {
            _db.Theatres.Update(theatre);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateScreenAsync(Screen screen)
        {
            _db.Screens.Update(screen);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateShowTimeAsync(ShowTime showTime)
        {
            _db.ShowTimes.Update(showTime);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateLanguageAsync(Language language)
        {
            _db.Languages.Update(language);
            await _db.SaveChangesAsync();
        }

        // ========== DELETE METHODS ==========

        public async Task DeleteMovieAsync(Movie movie)
        {
            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteTheatreAsync(Theatre theatre)
        {
            _db.Theatres.Remove(theatre);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteScreenAsync(Screen screen)
        {
            _db.Screens.Remove(screen);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteShowTimeAsync(ShowTime showTime)
        {
            _db.ShowTimes.Remove(showTime);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteLanguageAsync(Language language)
        {
            _db.Languages.Remove(language);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAdminAsync(User admin)
        {
            _db.Users.Remove(admin);
            await _db.SaveChangesAsync();
        }

        // ========== VALIDATION HELPERS ==========

        public async Task<bool> MovieHasActiveShowTimesAsync(Guid movieId)
        {
            return await _db.ShowTimes
                .AnyAsync(st => st.MovieId == movieId && st.IsActive);
        }

        public async Task<bool> TheatreHasActiveScreensAsync(Guid theatreId)
        {
            return await _db.Screens
                .AnyAsync(s => s.TheatreId == theatreId && s.IsActive);
        }

        public async Task<bool> ScreenHasActiveShowTimesAsync(Guid screenId)
        {
            return await _db.ShowTimes
                .AnyAsync(st => st.ScreenId == screenId && st.IsActive);
        }

        public async Task<bool> LanguageHasActiveShowTimesAsync(Guid languageId)
        {
            return await _db.ShowTimes
                .AnyAsync(st => st.LanguageId == languageId && st.IsActive);
        }

        public async Task<bool> AdminHasActiveTheatresAsync(Guid adminId)
        {
            return await _db.Theatres
                .AnyAsync(t => t.CreatedBy == adminId && t.IsActive);
        }

        // ========== CASCADE DELETE HELPERS ==========

        public async Task DeleteTheatreTimeSlotsAsync(Guid theatreId)
        {
            var timeSlots = await _db.TheatreTimeSlots
                .Where(ts => ts.TheatreId == theatreId)
                .ToListAsync();

            _db.TheatreTimeSlots.RemoveRange(timeSlots);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteScreenSeatsAsync(Guid screenId)
        {
            var seats = await _db.Seats
                .Where(s => s.ScreenId == screenId)
                .ToListAsync();

            _db.Seats.RemoveRange(seats);
            await _db.SaveChangesAsync();
        }

        public async Task<List<TheatreTimeSlot>> GetTheatreTimeSlotsAsync(Guid theatreId)
        {
            return await _db.TheatreTimeSlots
                .Where(ts => ts.TheatreId == theatreId)
                .ToListAsync();
        }

        public async Task<List<Seat>> GetScreenSeatsAsync(Guid screenId)
        {
            return await _db.Seats
                .Where(s => s.ScreenId == screenId)
                .ToListAsync();
        }
    }
}
