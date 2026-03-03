using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Constants;

namespace MovieBooking.Api.Controllers
{
    /// <summary>
    /// Controller for managing admin request approvals
    /// </summary>
    [ApiController]
    [Route("api/requestapproval")]
    [Authorize(Roles = "SuperAdmin")]
    public class RequestApprovalController : ControllerBase
    {
        private readonly IRequestApprovalService RequestApprovalService;

        /// <summary>Initializes a new instance of RequestApprovalController</summary>
        public RequestApprovalController(IRequestApprovalService requestApprovalService)
        {
            RequestApprovalService = requestApprovalService;
        }

        /// <summary>Retrieves all admin requests</summary>
        [HttpGet]
        public async Task<IActionResult> GetAllRequests()
        {
            try
            {
                var requests = await RequestApprovalService.GetAllRequestsAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = MessageStrings.ErrorRetrievingRequests,
                    error = ex.Message
                });
            }
        }

        /// <summary>Retrieves all pending admin requests</summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            try
            {
                var requests = await RequestApprovalService.GetAllPendingRequestsAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = MessageStrings.ErrorRetrievingPendingRequests,
                    error = ex.Message
                });
            }
        }

        /// <summary>Approves an admin request</summary>
        [HttpPut("{requestId}/approve")]
        public async Task<IActionResult> ApproveRequest(Guid requestId)
        {
            if (requestId == Guid.Empty)
                return BadRequest(new { message = MessageStrings.InvalidRequestId });

            try
            {
                await RequestApprovalService.ApproveRequestAsync(requestId);

                return Ok(new
                {
                    message = MessageStrings.RequestApprovedSuccessfully
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = MessageStrings.ErrorApprovingRequest,
                    error = ex.Message
                });
            }
        }

        /// <summary>Rejects an admin request</summary>
        [HttpPut("{requestId}/reject")]
        public async Task<IActionResult> RejectRequest(Guid requestId)
        {
            if (requestId == Guid.Empty)
                return BadRequest(new { message = MessageStrings.InvalidRequestId });

            try
            {
                await RequestApprovalService.RejectRequestAsync(requestId);

                return Ok(new
                {
                    message = MessageStrings.RequestRejectedSuccessfully
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = MessageStrings.ErrorRejectingRequest,
                    error = ex.Message
                });
            }
        }
    }
}