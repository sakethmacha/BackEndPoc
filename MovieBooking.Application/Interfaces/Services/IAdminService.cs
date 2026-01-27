using MovieBooking.Application.DTOs.Admin;

namespace MovieBooking.Application.Interfaces.Services
{
    public interface IAdminService
    {
        // Theatre Requests
        Task<Guid> RequestTheatreAsync(CreateTheatreRequestDto dto, Guid adminId);
        Task<List<TheatreRequestResponseDto>> GetMyTheatreRequestsAsync(Guid adminId);
        Task<List<TheatreRequestResponseDto>> GetMyApprovedTheatresAsync(Guid adminId);

        // Screen Requests
        Task<Guid> RequestScreenAsync(CreateScreenRequestDto dto, Guid adminId);
        Task<List<ScreenRequestResponseDto>> GetMyScreenRequestsAsync(Guid adminId);
        Task<List<ScreenRequestResponseDto>> GetMyApprovedScreensAsync(Guid adminId);

        // Get theatres for screen dropdown (only approved theatres of this admin)
        Task<List<TheatreRequestResponseDto>> GetMyTheatresForScreenAsync(Guid adminId);
    }
}