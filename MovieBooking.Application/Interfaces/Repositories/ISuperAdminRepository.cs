using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Domain.Entities;

namespace MovieBooking.Application.Interfaces.Repositories
{
    public interface ISuperAdminRepository
    {
        Task CreateAdminAsync(User user);
        Task<User> GetUserByIdAsync(Guid userId);
        Task UpdateUserAsync(User user);
        Task<List<AdminDto>> GetAdminsAsync();

        Task AddMovieAsync(Movie movie);
        Task<Movie> GetMovieByIdAsync(Guid movieId);
        Task UpdateMovieAsync(Movie movie);
        Task<List<TheatreTimeSlot>> GetTimeSlotsByTheatreAsync(Guid theatreId);
        Task AddShowTimesAsync(List<ShowTime> showTimes);

        Task<bool> ShowTimeConflictExistsAsync(
            Guid screenId,
            DateTime start,
            DateTime end);

        Task AddTheatreWithTimeSlotsAsync(Theatre theatre, List<TheatreTimeSlot> timeSlots);

        Task AddScreenAsync(Screen screen);
        //Task AddShowTimeAsync(ShowTime showTime);

        Task<AdminRequest> GetRequestByIdAsync(Guid requestId);
        Task UpdateRequestAsync(AdminRequest request);

        Task ApproveTheatreAsync(Guid theatreId);
        Task ApproveScreenAsync(Guid screenId);
        Task ApproveShowTimeAsync(Guid showTimeId);
        Task<Language?> GetLanguageByIdAsync(Guid languageId);

        //
        Task AddLanguageAsync(Language language);
        Task<List<Language>> GetLanguagesAsync();

        Task<bool> LanguageExistsAsync(string name);
        Task<List<Movie>> GetAllAsync();

        Task<List<Theatre>> GetTheatresAsync();
        Task<List<Screen>> GetScreensAsync();

        Task<List<ShowTime>> GetShowTimesAsync();
        Task<List<Screen>> GetByTheatreIdAsync(Guid theatreId);
        Task AddSeatsAsync(List<Seat> seats);


        // ========== GET BY ID METHODS ==========
        Task<Theatre> GetTheatreByIdAsync(Guid theatreId);
        Task<Screen> GetScreenByIdAsync(Guid screenId);
        Task<ShowTime> GetShowTimeByIdAsync(Guid showTimeId);
     

        // ========== UPDATE METHODS ==========
        Task UpdateTheatreAsync(Theatre theatre);
        Task UpdateScreenAsync(Screen screen);
        Task UpdateShowTimeAsync(ShowTime showTime);
        Task UpdateLanguageAsync(Language language);

        // ========== DELETE METHODS ==========
        Task DeleteMovieAsync(Movie movie);
        Task DeleteTheatreAsync(Theatre theatre);
        Task DeleteScreenAsync(Screen screen);
        Task DeleteShowTimeAsync(ShowTime showTime);
        Task DeleteLanguageAsync(Language language);
        Task DeleteAdminAsync(User admin);

        // ========== HELPER METHODS FOR DELETE VALIDATION ==========
        Task<bool> MovieHasActiveShowTimesAsync(Guid movieId);
        Task<bool> TheatreHasActiveScreensAsync(Guid theatreId);
        Task<bool> ScreenHasActiveShowTimesAsync(Guid screenId);
        Task<bool> LanguageHasActiveShowTimesAsync(Guid languageId);
        Task<bool> AdminHasActiveTheatresAsync(Guid adminId);

        // ========== CASCADE DELETE HELPERS ==========
        Task DeleteTheatreTimeSlotsAsync(Guid theatreId);
        Task DeleteScreenSeatsAsync(Guid screenId);
        Task<List<TheatreTimeSlot>> GetTheatreTimeSlotsAsync(Guid theatreId);
        Task<List<Seat>> GetScreenSeatsAsync(Guid screenId);


        //
        Task<List<AdminRequest>> GetAllPendingRequestsAsync();
        Task<List<AdminRequest>> GetAllRequestsAsync();

    }


}
