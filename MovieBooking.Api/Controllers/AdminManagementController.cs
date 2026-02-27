using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Services;

namespace MovieBooking.Api.Controllers
{
    /// <summary>
    /// Controller for managing admin user operations
    /// </summary>
    [ApiController]
    [Route("api/superadmin/admins")]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminManagementController : ControllerBase
    {
        private readonly IAdminManagementService _adminManagementService;

        /// <summary>
        /// Initializes a new instance of the AdminManagementController
        /// </summary>
        /// <param name="adminManagementService">Admin management service instance</param>
        public AdminManagementController(IAdminManagementService adminManagementService)
        {
            _adminManagementService = adminManagementService;
        }

        /// <summary>
        /// Retrieves all admin users
        /// </summary>
        /// <returns>List of admin users</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAdmins()
        {
            try
            {
                var admins = await _adminManagementService.GetAdminsAsync();
                return Ok(admins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving admins", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific admin user by ID
        /// </summary>
        /// <param name="adminId">Admin identifier</param>
        /// <returns>Admin user details</returns>
        [HttpGet("{adminId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404No