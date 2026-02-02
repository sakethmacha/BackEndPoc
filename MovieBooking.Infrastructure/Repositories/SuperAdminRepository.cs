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
        private readonly MovieBookingDatabaseContext DbContext;

        public SuperAdminRepository(MovieBookingDatabaseContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task CreateAdminAsync(User user)
        {
            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();
        }

        public Task<User> GetUserByIdAsync(Guid userId)
            => DbContext.Users.FindAsync(userId).AsTask();

        public async Task UpdateUserAsync(User user)
        {
            DbContext.Users.Update(user);
            await DbContext.SaveChangesAsync();
        }

        public async Task<List<AdminDto>> GetAdminsAsync()
        {
            return await DbContext.Users
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
            DbContext.Movies.Add(movie);
            await DbContext.SaveChangesAsync();
        }
        public async Task<List<Movie>> GetAllAsync()
        {
            return await DbContext.Movies
                .Where(m => m.IsActive)          
                .AsNoTracking()
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
        }

        public Task<Movie> GetMovieByIdAsync(Guid movieId)
            => DbContext.Movies.FindAsync(movieId).AsTask();

        public async Task UpdateMovieAsync(Movie movie)
        {
            DbContext.Movies.Update(movie);
            await DbContext.SaveChangesAsync();
        }

        public async Task AddTheatreAsync(Theatre theatre)
        {
            DbContext.Theatres.Add(theatre);
            await DbContext.SaveChangesAsync();
        }
        public async Task<List<Theatre>> GetTheatresAsync()
      => await DbContext.Theatres
          .Where(t => t.IsActive)     
          .AsNoTracking()
          .ToListAsync();

        public async Task AddScreenAsync(Screen screen)
        {
            DbContext.Screens.Add(screen);
            await DbContext.SaveChangesAsync();
        }

        public async Task<List<Screen>> GetScreensAsync()
       => await DbContext.Screens
        .Where(s => s.IsActive)     
        .AsNoTracking()
        .ToListAsync();


        public async Task AddSeatsAsync(List<Seat> seats)
        {
            DbContext.Seats.AddRange(seats);
            await DbContext.SaveChangesAsync();
        }


        public async Task<List<ShowTime>> GetShowTimesAsync()
        {
            return await DbContext.ShowTimes
                .Where(st => st.IsActive)          
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
            return await DbContext.TheatreTimeSlots
                .Where(t => t.TheatreId == theatreId && t.IsActive)
                .OrderBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task AddShowTimesAsync(List<ShowTime> showTimes)
        {
            DbContext.ShowTimes.AddRange(showTimes);
            await DbContext.SaveChangesAsync();
        }

        public Task<AdminRequest> GetRequestByIdAsync(Guid requestId)
            => DbContext.AdminRequests.FindAsync(requestId).AsTask();

        public async Task UpdateRequestAsync(AdminRequest adminRequest)
        {
            DbContext.AdminRequests.Update(adminRequest);
            await DbContext.SaveChangesAsync();
        }

        public async Task ApproveTheatreAsync(Guid theatreId)
        {
            var theatre = await DbContext.Theatres.FindAsync(theatreId);
            theatre.IsActive = true;
            theatre.ApprovalStatus = ApprovalStatus.APPROVED;
        }
        public async Task<List<Screen>> GetByTheatreIdAsync(Guid theatreId)
        {
            return await DbContext.Screens
                .Where(s => s.TheatreId == theatreId && s.IsActive)
                .OrderBy(s => s.ScreenName)
                .ToListAsync();
        }
        public async Task ApproveScreenAsync(Guid screenId)
        {
            var screen = await DbContext.Screens.FindAsync(screenId);
            screen.IsActive = true;
            screen.ApprovalStatus = ApprovalStatus.APPROVED;
        }
        public async Task<bool> ShowTimeConflictExistsAsync(
    Guid screenId, DateTime start, DateTime end)
        {
            return await DbContext.ShowTimes.AnyAsync(st =>
                st.ScreenId == screenId &&
                st.StartTime < end &&
                start < st.EndTime);
        }

        public async Task ApproveShowTimeAsync(Guid showTimeId)
        {
            var showTime = await DbContext.ShowTimes.FindAsync(showTimeId);
            showTime.IsActive = true;
            showTime.ApprovalStatus = ApprovalStatus.APPROVED;
        }
        public async Task AddTheatreWithTimeSlotsAsync(
    Theatre theatre,
    List<TheatreTimeSlot> timeSlots)
        {
            DbContext.Theatres.Add(theatre);
            DbContext.TheatreTimeSlots.AddRange(timeSlots);
            await DbContext.SaveChangesAsync();
        }

        public async Task AddLanguageAsync(Language language)
        {
            DbContext.Languages.Add(language);
            await DbContext.SaveChangesAsync();
        }

        public async Task<List<Language>> GetLanguagesAsync()
        {
            return await DbContext.Languages
                            .OrderBy(l => l.Name)
                            .ToListAsync();
        }
        public async Task<bool> LanguageExistsAsync(string name)
        {
            return await DbContext.Languages
                .AnyAsync(l => l.Name.ToLower() == name.ToLower());
        }
        public async Task<Theatre> GetTheatreByIdAsync(Guid theatreId)
        {
            var theatre = await DbContext.Theatres
                .Include(t => t.TimeSlots)
                .FirstOrDefaultAsync(t => t.TheatreId == theatreId);

            if (theatre == null)
                throw new InvalidOperationException("Theatre not found");

            return theatre;
        }

        public async Task<Screen> GetScreenByIdAsync(Guid screenId)
        {
            var screen = await DbContext.Screens
                .Include(s => s.Seats)
                .FirstOrDefaultAsync(s => s.ScreenId == screenId);

            if (screen == null)
                throw new InvalidOperationException("Screen not found");

            return screen;
        }
        public async Task DeleteTheatreTimeSlotsAsync(Guid theatreId) 
        { 
            var timeSlots = await DbContext.TheatreTimeSlots.Where(ts => ts.TheatreId == theatreId).ToListAsync(); 
            DbContext.TheatreTimeSlots.RemoveRange(timeSlots);
            await DbContext.SaveChangesAsync(); 
        }
        public async Task DeleteScreenSeatsAsync(Guid screenId) 
        {
            var seats = await DbContext.Seats.Where(s => s.ScreenId == screenId).ToListAsync(); 
            DbContext.Seats.RemoveRange(seats); 
            await DbContext.SaveChangesAsync(); 
        }
        public async Task<ShowTime> GetShowTimeByIdAsync(Guid showTimeId)
        {
            var showTime = await DbContext.ShowTimes
                .Include(s => s.Movie)
                .Include(s => s.Theatre)
                .Include(s => s.Screen)
                .Include(s => s.Language)
                .FirstOrDefaultAsync(s => s.ShowTimeId == showTimeId);

            if (showTime == null)
                throw new InvalidOperationException("ShowTime not found");

            return showTime;
        }

        public async Task<Language> GetLanguageByIdAsync(Guid languageId)
        {
            var language = await DbContext.Languages.FindAsync(languageId);

            if (language == null)
                throw new InvalidOperationException("Language not found");

            return language;
        }

        public async Task UpdateTheatreAsync(Theatre theatre)
        {
            DbContext.Theatres.Update(theatre);
            await DbContext.SaveChangesAsync();
        }

        public async Task UpdateScreenAsync(Screen screen)
        {
            DbContext.Screens.Update(screen);
            await DbContext.SaveChangesAsync();
        }

        public async Task UpdateShowTimeAsync(ShowTime showTime)
        {
            DbContext.ShowTimes.Update(showTime);
            await DbContext.SaveChangesAsync();
        }

        public async Task UpdateLanguageAsync(Language language)
        {
            DbContext.Languages.Update(language);
            await DbContext.SaveChangesAsync();
        }

        public async Task DeleteMovieAsync(Movie movie)
        {
            if (await MovieHasActiveShowTimesAsync(movie.MovieId))
            {
                throw new InvalidOperationException(
                    "Cannot deactivate movie while active showtimes exist.");
            }

            movie.IsActive = false;
            await DbContext.SaveChangesAsync();
        }

        public async Task DeleteTheatreAsync(Theatre theatre)
        {
            if (await TheatreHasActiveScreensAsync(theatre.TheatreId))
            {
                throw new InvalidOperationException(
                    "Cannot deactivate theatre while active screens exist.");
            }

            theatre.IsActive = false;
            await DbContext.SaveChangesAsync();
        }

        public async Task DeleteScreenAsync(Screen screen)
        {
            if (await ScreenHasActiveShowTimesAsync(screen.ScreenId))
            {
                throw new InvalidOperationException(
                    "Cannot deactivate screen while active showtimes exist.");
            }

            // deactivate screen
            screen.IsActive = false;

           
            foreach (var seat in screen.Seats)
            {
                seat.IsActive = false;
            }

            await DbContext.SaveChangesAsync();
        }

        public async Task DeleteShowTimeAsync(ShowTime showTime)
        {
            showTime.IsActive = false;
            await DbContext.SaveChangesAsync();
        }

        public async Task DeleteLanguageAsync(Language language)
        {
            DbContext.Languages.Remove(language);
            await DbContext.SaveChangesAsync();
        }

        public async Task DeleteAdminAsync(User admin)
        {
            if (await AdminHasActiveTheatresAsync(admin.UserId))
            {
                throw new InvalidOperationException(
                    "Cannot deactivate admin while active theatres exist.");
            }

            admin.IsActive = false;
            await DbContext.SaveChangesAsync();
        }

        public async Task<bool> MovieHasActiveShowTimesAsync(Guid movieId)
        {
            return await DbContext.ShowTimes
                .AnyAsync(st => st.MovieId == movieId && st.IsActive);
        }

        public async Task<bool> TheatreHasActiveScreensAsync(Guid theatreId)
        {
            return await DbContext.Screens
                .AnyAsync(s => s.TheatreId == theatreId && s.IsActive);
        }

        public async Task<bool> ScreenHasActiveShowTimesAsync(Guid screenId)
        {
            return await DbContext.ShowTimes
                .AnyAsync(st => st.ScreenId == screenId && st.IsActive);
        }

        public async Task<bool> LanguageHasActiveShowTimesAsync(Guid languageId)
        {
            return await DbContext.ShowTimes
                .AnyAsync(st => st.LanguageId == languageId && st.IsActive);
        }

        public async Task<bool> AdminHasActiveTheatresAsync(Guid adminId)
        {
            return await DbContext.Theatres
                .AnyAsync(t => t.CreatedBy == adminId && t.IsActive);
        }

        public async Task<List<TheatreTimeSlot>> GetTheatreTimeSlotsAsync(Guid theatreId)
        {
            return await DbContext.TheatreTimeSlots
                .Where(ts => ts.TheatreId == theatreId)
                .ToListAsync();
        }

        public async Task<List<Seat>> GetScreenSeatsAsync(Guid screenId)
        {
            return await DbContext.Seats
                .Where(s => s.ScreenId == screenId)
                .ToListAsync();
        }
        //
        public async Task<List<AdminRequest>> GetAllPendingRequestsAsync()
        {
            return await DbContext.AdminRequests
                .Where(r => r.Status == ApprovalStatus.PENDING)
                .Include(r => r.RequestedByUser)
                .OrderBy(r => r.RequestedAt)
                .ToListAsync();
        }

        public async Task<List<AdminRequest>> GetAllRequestsAsync()
        {
            return await DbContext.AdminRequests
                .Include(r => r.RequestedByUser)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();
        }
    }
}
