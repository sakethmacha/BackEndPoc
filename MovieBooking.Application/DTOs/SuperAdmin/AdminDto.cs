namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class AdminDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
}
