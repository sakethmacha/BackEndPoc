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
    [Route("api/adminmanagement")]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminManagementController : ControllerBase
    {
        private readonly IAdminManagementService AdminManagementService;

        /// <summary>Initializes a new instance of AdminManagementController</summary>
        public AdminManagementController(IAdminManagementService adminManagementService)
        {
            AdminManagementService = adminManagementService;
        }

        /// <summary>Retrieves all admin users</summary>
        [HttpGet]
        public async Task<IActionResult> GetAdmins()
        {
            try
            {
                var admins = await AdminManagementService.GetAdminsAsync();
                return Ok(admins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving admins", error = ex.Message });
            }
        }

        /// <summary>Retrieves an admin by ID</summary>
        [HttpGet("{adminId}")]
        public async Task<IActionResult> GetAdminById(Guid adminId)
        {
            if (adminId == Guid.Empty)
                return BadRequest(new { message = "Invalid admin ID" });
            try
            {
                var admin = await AdminManagementService.GetAdminByIdAsync(adminId);
                return Ok(admin);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the admin", error = ex.Message });
            }
        }

        /// <summary>Creates a new admin user</summary>
        [HttpPost]
        public async Task<IActionResult> CreateAdmin(CreateAdminDto createAdminDto)
        {
            try
            {
                await AdminManagementService.CreateAdminAsync(createAdminDto);
                return Ok(new { message = "Admin created successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the admin", error = ex.Message });
            }
        }

        /// <summary>Toggles admin active status</summary>
        [HttpPut("{adminId}/toggle")]
        public async Task<IActionResult> ToggleAdmin(Guid adminId)
        {
            if (adminId == Guid.Empty)
                return BadRequest(new { message = "Invalid admin ID" });
            try
            {
                await AdminManagementService.ToggleAdminAsync(adminId);
                return Ok(new { message = "Admin status toggled successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while toggling the admin", error = ex.Message });
            }
        }

        /// <summary>Updates an existing admin</summary>
        [HttpPut("{adminId}")]
        public async Task<IActionResult> UpdateAdmin(Guid adminId, [FromBody] UpdateAdminDto updateAdminDto)
        {
            if (adminId == Guid.Empty)
                return BadRequest(new { message = "Invalid admin ID" });
            try
            {
                await AdminManagementService.UpdateAdminAsync(adminId, updateAdminDto);
                return Ok(new { message = "Admin updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the admin", error = ex.Message });
            }
        }

        /// <summary>Deletes an admin</summary>
        [HttpDelete("{adminId}")]
        public async Task<IActionResult> DeleteAdmin(Guid adminId)
        {
            if (adminId == Guid.Empty)
                return BadRequest(new { message = "Invalid admin ID" });
            try
            {
                await AdminManagementService.DeleteAdminAsync(adminId);
                return Ok(new { message = "Admin deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the admin", error = ex.Message });
            }
        }
    }
}