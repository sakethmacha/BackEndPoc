using MovieBooking.Domain.Entities;

namespace MovieBooking.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for theatre data access operations
    /// </summary>
    public interface ITheatreRepository
    {
        /// <summary>
        /// Retrieves all active theatres
        /// </summary>
        /// <returns>List of theatres</returns>
        Task<List<Theatre>> GetAllTheatresAsync();

        /// <summary>
        /// Retrieves a theatre by ID with time slots
        /// </summary>
        /// <param name="theatreId">Theatre identifier</param>
        /// <returns>Theatre entity with time slots</returns>
        Task<Theatre> GetTheatreByIdAsync(Guid theatreId);

        /// <summary>
        /// Adds a new theatre with time slots
        /// </summary>
        /// <param name="theatre">Theatre entity</param>
        /// <param name="timeSlots">List of time slots</param>
        Task AddTheatreWithTimeSlotsAsync(Theatre theatre, List<TheatreTimeSlot> timeSlots);

        /// <summary>
        /// Updates an existing theatre
        /// </summary>
        /// <param name="theatre">Theatre entity</param>
        Task UpdateTheatreAsync(Theatre theatre);

        /// <summary>
        /// Deletes a theatre (soft delete by setting IsActive = false)
        /// </summary>
        /// <param name="theatre">Theatre entity</param>
        Task DeleteTheatreAsync(Theatre theatre);

        /// <summary>
        /// Retrieves time slots for a theatre
        /// </summary>
        /// <param name="theatreId">Theatre identifier</param>
        /// <returns>List of time slots</returns>
        Task<List<TheatreTimeSlot>> GetTimeSlotsByTheatreAsync(Guid theatreId);

        /// <summary>
        /// Deletes all time slots for a theatre
        /// </summary>
        /// <param name="theatreId">Theatre identifier</param>
        Task DeleteTheatreTimeSlotsAsync(Guid theatreId);

        /// <summary>
        /// Checks if a theatre has active screens
        /// </summary>
        /// <param name="theatreId">Theatre identifier</param>
        /// <returns>True if theatre has active screens</returns>
        Task<bool> TheatreHasActiveScreensAsync(Guid theatreId);
    }
}