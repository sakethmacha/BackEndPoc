using MovieBooking.Domain.Entities;

namespace MovieBooking.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for theatre data access operations
    /// </summary>
    public interface ITheatreRepository
    {
        /// <summary>Retrieves all active theatres</summary>
        Task<List<Theatre>> GetTheatresAsync();

        /// <summary>Retrieves a theatre by ID with time slots</summary>
        Task<Theatre> GetTheatreByIdAsync(Guid theatreId);

        /// <summary>Adds a theatre with its time slots</summary>
        Task AddTheatreWithTimeSlotsAsync(Theatre theatre, List<TheatreTimeSlot> timeSlots);

        /// <summary>Updates an existing theatre</summary>
        Task UpdateTheatreAsync(Theatre theatre);

        /// <summary>Soft deletes a theatre</summary>
        Task DeleteTheatreAsync(Theatre theatre);

        /// <summary>Retrieves time slots for a theatre</summary>
        Task<List<TheatreTimeSlot>> GetTimeSlotsByTheatreAsync(Guid theatreId);

        /// <summary>Retrieves all time slots for a theatre</summary>
        Task<List<TheatreTimeSlot>> GetTheatreTimeSlotsAsync(Guid theatreId);

        /// <summary>Deletes all time slots for a theatre</summary>
        Task DeleteTheatreTimeSlotsAsync(Guid theatreId);

        /// <summary>Checks if theatre has active screens</summary>
        Task<bool> TheatreHasActiveScreensAsync(Guid theatreId);

        /// <summary>Approves a theatre</summary>
        Task ApproveTheatreAsync(Guid theatreId);

        /// <summary>Rejects a theatre</summary>
        Task RejectTheatreAsync(Guid theatreId);
    }
}