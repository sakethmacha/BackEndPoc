namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class UpdateAdminDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; } // Optional - only if changing password
    }
}
