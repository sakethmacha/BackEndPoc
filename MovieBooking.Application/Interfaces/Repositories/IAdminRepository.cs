using MovieBooking.Domain.Entities;
using MovieBooking.Application.DTOs.Admin;

namespace MovieBooking.Application.Interfaces.Repositories
{
    public interface IAdminRepository
    {
        // Theatre Management
        Task<Guid> CreateTheatreRequestAsync(Theatre theatre, List<TheatreTimeSlot> timeSlots, AdminRequest request);
        Task<List<Theatre>> GetTheatresByAdminAsync(Guid adminId);
        Task<Theatre> GetTheatreByIdAsync(Guid theatreId);

        // Screen Management
        Task<Guid> CreateScreenRequestAsync(Screen screen, List<Seat> seats, AdminRequest request);
        Task<List<Screen>> GetScreensByAdminAsync(Guid adminId);
        Task<Screen> GetScreenByIdAsync(Guid screenId);

        // Requests
        Task<List<AdminRequest>> GetRequestsByAdminAsync(Guid adminId);
        Task<AdminRequest> GetRequestByIdAsync(Guid requestId);
    }
}