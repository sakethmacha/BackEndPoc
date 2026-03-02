using MovieBooking.Application.DTOs.Admin;
using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for admin request approval operations
    /// </summary>
    public interface IRequestApprovalService
    {
        /// <summary>Retrieves all admin requests</summary>
        Task<List<AdminRequestResponseDto>> GetAllRequestsAsync();

        /// <summary>Retrieves all pending admin requests</summary>
        Task<List<AdminRequestDto>> GetAllPendingRequestsAsync();

        /// <summary>Approves an admin request</summary>
        Task ApproveRequestAsync(Guid requestId);

        /// <summary>Rejects an admin request</summary>
        Task RejectRequestAsync(Guid requestId);
    }
}