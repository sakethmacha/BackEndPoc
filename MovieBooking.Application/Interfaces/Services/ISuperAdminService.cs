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

        // ========== UPDATE METHODS ==========
        Task UpdateMovieAsync(Guid movieId, UpdateMovieDto dto);
        Task UpdateTheatreAsync(Guid theatreId, UpdateTheatreDto dto);
        Task UpdateScreenAsync(Guid screenId, UpdateScreenDto dto);
        Task UpdateShowTimeAsync(Guid showTimeId, UpdateShowTimeDto dto);
        Task UpdateLanguageAsync(Guid languageId, UpdateLanguageDto dto);
        Task UpdateAdminAsync(Guid adminId, UpdateAdminDto dto);

        // ========== DELETE METHODS ==========
        Task DeleteMovieAsync(Guid movieId);
        Task DeleteTheatreAsync(Guid theatreId);
        Task DeleteScreenAsync(Guid screenId);
        Task DeleteShowTimeAsync(Guid showTimeId);
        Task DeleteLanguageAsync(Guid languageId);
        Task DeleteAdminAsync(Guid adminId);

        Task<MovieResponse> GetMovieByIdAsync(Guid movieId);
        Task<TheatreResponseDto> GetTheatreByIdAsync(Guid theatreId);
        Task<CreateScreenRequest> GetScreenByIdAsync(Guid screenId);
        Task<ShowTimeResponseDto> GetShowTimeByIdAsync(Guid showTimeId);
        Task<LanguageDto> GetLanguageByIdAsync(Guid languageId);
        Task<AdminDto> GetAdminByIdAsync(Guid adminId);

    }
}


