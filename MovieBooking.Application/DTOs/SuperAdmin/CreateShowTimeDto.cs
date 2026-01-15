namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class CreateShowTimeDto
    {
        public Guid MovieId { get; set; }
        public Guid TheatreId { get; set; }
        public Guid ScreenId { get; set; }

        public Guid LanguageId { get; set; }

        public DateOnly ShowDate { get; set; }
        public decimal BasePrice { get; set; }
    }
}
