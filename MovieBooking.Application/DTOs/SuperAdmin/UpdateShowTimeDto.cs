namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class UpdateShowTimeDto
    {
        public Guid MovieId { get; set; }
        public Guid LanguageId { get; set; }
        public DateOnly ShowDate { get; set; }
        public decimal BasePrice { get; set; }
    }
}
