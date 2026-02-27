using MovieBooking.Application.DTOs.Admin;
using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for admin request approval operations
    /// </summary>
    public interface IRequestApprovalService
    {
        /// <summary>
        /// Retrieves all admin requests
        /// </summary>
        /// <returns>List of all requests</returns>
        Task<List<AdminRequestResponseDto>> GetAllRequestsAsync();

        /// <summary>
        /// Retrieves only pending admin requests
        /// </summary>
        /// <returns>List of pending requests</returns>
        Task<List<AdminRequestDto>> GetPendingRequestsAsync();

        /// <summary>
        /// Approves an admin request
        /// </summary>
        /// <param name="requestId">Request identifier</param>
        Task ApproveRequestAsync(Guid requestId);

        /// <summary>
        /// Rejects an admin request
        /// </summary>
        /// <param name="requestId">Request identifier</param>
        Task RejectRequestAsync(Guid requestId);
    }
}