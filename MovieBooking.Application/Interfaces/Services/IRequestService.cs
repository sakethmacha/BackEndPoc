using MovieBooking.Application.DTOs.Admin;

namespace MovieBooking.Application.Interfaces.Services
{
    public interface IRequestService
    {
        // Theatre Requests
        Task<Guid> RequestTheatreAsync(CreateTheatreRequestDto dto, Guid adminId);
        Task<List<TheatreRequestResponseDto>> GetTheatreRequestsAsync(Guid adminId);
        Task<List<TheatreRequestResponseDto>> GetApprovedTheatresAsync(Guid adminId);

        // Screen Requests
        Task<Guid> RequestScreenAsync(CreateScreenRequestDto dto, Guid adminId);
        Task<List<ScreenRequestResponseDto>> GetScreenRequestsAsync(Guid adminId);
        Task<List<ScreenRequestResponseDto>> GetApprovedScreensAsync(Guid adminId);

        // Get theatres for screen dropdown (only approved theatres of this admin)
        Task<List<TheatreRequestResponseDto>> GetTheatresForScreenAsync(Guid adminId);
    }
}