using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.Interfaces.Services;

namespace MovieBooking.Api.Controllers
{
    /// <summary>
    /// Controller for managing admin request approvals
    /// </summary>
    [ApiController]
    [Route("api/superadmin/requests")]
    [Authorize(Roles = "SuperAdmin")]
    public class RequestApprovalController : ControllerBase
    {
        private readonly IRequestApprovalService _requestApprovalService;

        /// <summary>
        /// Initializes a new instance of the RequestApprovalController
        /// </summary>
        /// <param name="requestApprovalService">Request approval service instance</param>
        public RequestApprovalController(IRequestApprovalService requestApprovalService)
        {
            _requestApprovalService = requestApprovalService;
        }

        /// <summary>
        /// Retrieves all admin requests
        /// </summary>
        /// <returns>List of all requests</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllRequests()
        {
            try
            {
                var requests = await _requestApprovalService.GetAllRequestsAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving requests", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all pending admin requests
        /// </summary>
        /// <returns>List of pending requests</returns>
        [HttpGet("pending")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPendingRequests()
        {
            try
            {
                var requests = await _requestApprovalService.GetPendingRequestsAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving pending requests", error = ex.Message });
            }
        }

        /// <summary>
        /// Approves an admin request
        /// </summary>
        /// <param name="requestId">Request identifier</param>
        /// <returns>Success message</returns>
        [HttpPut("{requestId}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveRequest(Guid requestId)
        {
            if (requestId == Guid.Empty)
                return BadRequest(new { message = "Invalid request ID" });

            try
            {
                await _requestApprovalService.ApproveRequestAsync(requestId);
                return Ok(new { message = "Request approved successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while approving the request", error = ex.Message });
            }
        }

        /// <summary>
        /// Rejects an admin request
        /// </summary>
        /// <param name="requestId">Request identifier</param>
        /// <returns>Success message</returns>
        [HttpPut("{requestId}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectRequest(Guid requestId)
        {
            if (requestId == Guid.Empty)