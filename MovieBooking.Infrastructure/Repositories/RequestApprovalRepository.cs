using Microsoft.EntityFrameworkCore;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;
using MovieBooking.Infrastructure.Persistence;

namespace MovieBooking.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for admin request approval data access operations
    /// </summary>
    public class RequestApprovalRepository : IRequestApprovalRepository
    {
        private readonly MovieBookingDatabaseContext DbContext;

        /// <summary>Initializes a new instance of RequestApprovalRepository</summary>
        public RequestApprovalRepository(MovieBookingDatabaseContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<List<AdminRequest>> GetAllRequestsAsync()
            => await DbContext.AdminRequests
                .Include(r => r.RequestedByUser)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<List<AdminRequest>> GetAllPendingRequestsAsync()
            => await DbContext.AdminRequests
                .Where(r => r.Status == ApprovalStatus.PENDING)
                .Include(r => r.RequestedByUser)
                .OrderBy(r => r.RequestedAt)
                .ToListAsync();

        /// <inheritdoc/>
        public Task<AdminRequest> GetRequestByIdAsync(Guid requestId)
            => DbContext.AdminRequests.FindAsync(requestId).AsTask();

        /// <inheritdoc/>
        public async Task UpdateRequestAsync(AdminRequest adminRequest)
        {
            DbContext.AdminRequests.Update(adminRequest);
            await DbContext.SaveChangesAsync();
        }
    }
}