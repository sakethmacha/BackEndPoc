using Microsoft.EntityFrameworkCore;
using MovieBooking.Application.DTOs.Admin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;
using MovieBooking.Infrastructure.Persistence;

namespace MovieBooking.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly MovieBookingDatabaseContext _db;

        public AdminRepository(MovieBookingDatabaseContext db)
        {
            _db = db;
        }

        // ========== THEATRE MANAGEMENT ==========

        public async Task<Guid> CreateTheatreRequestAsync(Theatre theatre, List<TheatreTimeSlot> timeSlots, AdminRequest request)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Add theatre with PENDING status
                _db.Theatres.Add(theatre);
                await _db.SaveChangesAsync();

                // Add time slots
                _db.TheatreTimeSlots.AddRange(timeSlots);
                await _db.SaveChangesAsync();

                // Create approval request
                request.ReferenceId = theatre.TheatreId;
                _db.AdminRequests.Add(request);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                return theatre.TheatreId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Theatre>> GetTheatresByAdminAsync(Guid adminId)
        {
            return await _db.Theatres
                .Include(t => t.TimeSlots)
                .Where(t => t.CreatedBy == adminId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Theatre> GetTheatreByIdAsync(Guid theatreId)
        {
            var theatre = await _db.Theatres
                .Include(t => t.TimeSlots)
                .FirstOrDefaultAsync(t => t.TheatreId == theatreId);

            if (theatre == null)
                throw new InvalidOperationException("Theatre not found");

            return theatre;
        }

        // ========== SCREEN MANAGEMENT ==========

        public async Task<Guid> CreateScreenRequestAsync(Screen screen, List<Seat> seats, AdminRequest request)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Add screen with PENDING status
                _db.Screens.Add(screen);
                await _db.SaveChangesAsync();

                // Add seats
                _db.Seats.AddRange(seats);
                await _db.SaveChangesAsync();

                // Create approval request
                request.ReferenceId = screen.ScreenId;
                _db.AdminRequests.Add(request);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                return screen.ScreenId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Screen>> GetScreensByAdminAsync(Guid adminId)
        {
            return await _db.Screens
                .Include(s => s.Theatre)
                .Include(s => s.Seats)
                .Where(s => s.Theatre.CreatedBy == adminId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<Screen> GetScreenByIdAsync(Guid screenId)
        {
            var screen = await _db.Screens
                .Include(s => s.Theatre)
                .Include(s => s.Seats)
                .FirstOrDefaultAsync(s => s.ScreenId == screenId);

            if (screen == null)
                throw new InvalidOperationException("Screen not found");

            return screen;
        }

        // ========== REQUESTS ==========

        public async Task<List<AdminRequest>> GetRequestsByAdminAsync(Guid adminId)
        {
            return await _db.AdminRequests
                .Where(r => r.RequestedBy == adminId)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();
        }

        public async Task<AdminRequest> GetRequestByIdAsync(Guid requestId)
        {
            var request = await _db.AdminRequests.FindAsync(requestId);
            if (request == null)
                throw new InvalidOperationException("Request not found");

            return request;
        }
    }
}