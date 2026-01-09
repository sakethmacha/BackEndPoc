namespace MovieBooking.Domain.Entities
{
    public class Language
    {
        public Guid LanguageId { get; set; }
        public string Name { get; set; }

        public ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
    }

}
