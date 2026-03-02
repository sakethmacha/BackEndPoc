using MovieBooking.Domain.Entities;

namespace MovieBooking.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for admin request approval data access operations
    /// </summary>
    public interface IRequestApprovalRepository
    {
        /// <summary>Retrieves all admin requests with requesting user</summary>
        Task<List<AdminRequest>> GetAllRequestsAsync();

        /// <summary>Retrieves all pending requests with requesting user</summary>
        Task<List<AdminRequest>> GetAllPendingRequestsAsync();

        /// <summary>Retrieves a request by ID</summary>
        Task<AdminRequest> GetRequestByIdAsync(Guid requestId);

        /// <summary>Updates a request (approve/reject)</summary>
        Task UpdateRequestAsync(AdminRequest adminRequest);
    }
}