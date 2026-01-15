using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Domain.Entities;
namespace MovieBooking.Application.Interfaces.Services
{
    using MovieBooking.Application.DTOs.SuperAdmin;

    public interface ISuperAdminService
    {
        Task CreateAdminAsync(CreateAdminDto dto);
        Task<List<AdminDto>> GetAdminsAsync();
        Task ToggleAdminAsync(Guid adminId);

        Task AddMovieAsync(AddMovieDto dto);
        Task ToggleMovieAsync(Guid movieId);

        Task AddTheatreAsync(CreateTheatreDto dto, Guid superAdminId);
        Task AddScreenAsync(CreateScreenRequest dto);
        Task AddShowTimeAsync(CreateShowTimeDto dto);

        Task ApproveRequestAsync(Guid requestId);
        Task RejectRequestAsync(Guid requestId);

        Task AddLanguageAsync(CreateLanguageDto dto);
        Task<List<LanguageDto>> GetLanguagesAsync();
        Task<List<MovieResponse>> GetMoviesAsync();
        Task<List<TheatreResponseDto>> GetTheatresAsync();
        Task<List<ScreenResponseDto>> GetScreensAsync();
        Task<List<ShowTimeResponseDto>> GetShowTimesAsync();

        Task<List<ScreenResponseDto>> GetScreensByTheatreAsync(Guid theatreId);
    }
}


