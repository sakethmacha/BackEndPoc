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

        Task AddTheatreAsync(Theatre theatre);
        Task AddScreenAsync(Screen screen);
        Task AddShowTimeAsync(ShowTime showTime);

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
    }


}
