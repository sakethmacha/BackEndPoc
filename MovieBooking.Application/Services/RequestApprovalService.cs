using MovieBooking.Application.DTOs.Admin;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;

namespace MovieBooking.Application.Services
{
    /// <summary>
    /// Service implementation for admin request approval operations
    /// </summary>
    public class RequestApprovalService : IRequestApprovalService
    {
        private readonly IRequestApprovalRepository RequestApprovalRepository;
        private readonly ITheatreRepository TheatreRepository;
        private readonly IScreenRepository ScreenRepository;

        /// <summary>Initializes a new instance of RequestApprovalService</summary>
        public RequestApprovalService(
            IRequestApprovalRepository requestApprovalRepository,
            ITheatreRepository theatreRepository,
            IScreenRepository screenRepository)
        {
            RequestApprovalRepository = requestApprovalRepository;
            TheatreRepository = theatreRepository;
            ScreenRepository = screenRepository;
        }

        /// <inheritdoc/>
        public async Task<List<AdminRequestResponseDto>> GetAllRequestsAsync()
        {
            var requests = await RequestApprovalRepository.GetAllRequestsAsync();
            return requests.Select(r => new AdminRequestResponseDto
            {
                AdminRequestId = r.AdminRequestId,
                RequestType = r.RequestType.ToString(),
                Status = r.Status.ToString(),
                RequestedAt = r.RequestedAt,
                ReviewedAt = r.ReviewedAt,
                RequestedBy = r.RequestedByUser.Name
            }).ToList();
        }

        /// <inheritdoc/>
        public async Task<List<AdminRequestDto>> GetAllPendingRequestsAsync()
        {
            var requests = await RequestApprovalRepository.GetAllPendingRequestsAsync();
            return requests.Select(r => new AdminRequestDto
            {
                AdminRequestId = r.AdminRequestId,
                RequestType = r.RequestType.ToString(),
                Status = r.Status.ToString(),
                RequestedAt = r.RequestedAt,
                ReviewedAt = r.ReviewedAt,
                RequestDetails = GetRequestDetails(r)
            }).ToList();
        }

        /// <inheritdoc/>
        public async Task ApproveRequestAsync(Guid requestId)
        {
            var request = await RequestApprovalRepository.GetRequestByIdAsync(requestId);
            request.Status = ApprovalStatus.APPROVED;
            request.ReviewedAt = DateTime.UtcNow;

            switch (request.RequestType)
            {
                case RequestType.THEATRE:
                    await TheatreRepository.ApproveTheatreAsync(request.ReferenceId);
                    break;
                case RequestType.SCREEN:
                    await ScreenRepository.ApproveScreenAsync(request.ReferenceId);
                    break;
            }

            await RequestApprovalRepository.UpdateRequestAsync(request);
        }

        /// <inheritdoc/>
        public async Task RejectRequestAsync(Guid requestId)
        {
            var request = await RequestApprovalRepository.GetRequestByIdAsync(requestId);
            request.Status = ApprovalStatus.REJECTED;
            request.ReviewedAt = DateTime.UtcNow;

            switch (request.RequestType)
            {
                case RequestType.THEATRE:
                    await TheatreRepository.RejectTheatreAsync(request.ReferenceId);
                    break;
                case RequestType.SCREEN:
                    await ScreenRepository.RejectScreenAsync(request.ReferenceId);
                    break;
            }

            await RequestApprovalRepository.UpdateRequestAsync(request);
        }

        private string GetRequestDetails(AdminRequest request)
            => $"{request.RequestType} - Reference ID: {request.ReferenceId}";
    }
}