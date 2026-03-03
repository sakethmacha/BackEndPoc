using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Constants;
using MovieBooking.Domain.Entities;

namespace MovieBooking.Application.Services
{
    /// <summary>
    /// Service implementation for showtime management operations
    /// </summary>
    public class ShowTimeService : IShowTimeService
    {
        private readonly IShowTimeRepository ShowTimeRepository;
        private readonly ITheatreRepository TheatreRepository;

        /// <summary>Initializes a new instance of ShowTimeService</summary>
        public ShowTimeService(
            IShowTimeRepository showTimeRepository,
            ITheatreRepository theatreRepository)
        {
            ShowTimeRepository = showTimeRepository;
            TheatreRepository = theatreRepository;
        }

        /// <inheritdoc/>
        public async Task<List<ShowTimeResponseDto>> GetShowTimesAsync()
        {
            var showTimes = await ShowTimeRepository.GetShowTimesAsync();

            return showTimes.Select(st => new ShowTimeResponseDto
            {
                ShowTimeId = st.ShowTimeId,
                MovieTitle = st.Movie.Title,
                TheatreName = st.Theatre.Name,
                ScreenName = st.Screen.ScreenName,
                LanguageName = st.Language.Name,
                StartTime = st.StartTime,
                BasePrice = st.BasePrice
            }).ToList();
        }

        /// <inheritdoc/>
        public async Task<ShowTimeResponseDto> GetShowTimeByIdAsync(Guid showTimeId)
        {
            var showTime = await ShowTimeRepository.GetShowTimeByIdAsync(showTimeId);

            return new ShowTimeResponseDto
            {
                ShowTimeId = showTime.ShowTimeId,
                MovieTitle = showTime.Movie.Title,
                TheatreName = showTime.Theatre.Name,
                ScreenName = showTime.Screen.ScreenName,
                LanguageName = showTime.Language.Name,
                StartTime = showTime.StartTime,
                BasePrice = showTime.BasePrice
            };
        }

        /// <inheritdoc/>
        public async Task AddShowTimeAsync(CreateShowTimeDto createShowTimeDto)
        {
            var slots = await TheatreRepository
                .GetTimeSlotsByTheatreAsync(createShowTimeDto.TheatreId);

            if (!slots.Any())
                throw new InvalidOperationException(
                    MessageStrings.TheatreHasNoConfiguredTimeSlots);

            var showTimes = new List<ShowTime>();

            foreach (var slot in slots)
            {
                var start = createShowTimeDto.ShowDate.ToDateTime(slot.StartTime);
                var end = createShowTimeDto.ShowDate.ToDateTime(slot.EndTime);

                bool conflict = await ShowTimeRepository
                    .ShowTimeConflictExistsAsync(
                        createShowTimeDto.ScreenId,
                        start,
                        end);

                if (conflict)
                    throw new InvalidOperationException(
                        MessageStrings.ScreenAlreadyScheduledForSelectedDate);

                showTimes.Add(new ShowTime
                {
                    ShowTimeId = Guid.NewGuid(),
                    TheatreId = createShowTimeDto.TheatreId,
                    ScreenId = createShowTimeDto.ScreenId,
                    MovieId = createShowTimeDto.MovieId,
                    LanguageId = createShowTimeDto.LanguageId,
                    StartTime = start,
                    EndTime = end,
                    BasePrice = createShowTimeDto.BasePrice,
                    IsActive = true
                });
            }

            await ShowTimeRepository.AddShowTimesAsync(showTimes);
        }

        /// <inheritdoc/>
        public async Task UpdateShowTimeAsync(
            Guid showTimeId,
            UpdateShowTimeDto updateShowTimeDto)
        {
            var showTime =
                await ShowTimeRepository.GetShowTimeByIdAsync(showTimeId);

            var slots =
                await TheatreRepository.GetTimeSlotsByTheatreAsync(
                    showTime.TheatreId);

            if (!slots.Any())
                throw new InvalidOperationException(
                    MessageStrings.TheatreHasNoConfiguredTimeSlots);

            var slot = slots.First();

            var start =
                updateShowTimeDto.ShowDate.ToDateTime(slot.StartTime);

            var end =
                updateShowTimeDto.ShowDate.ToDateTime(slot.EndTime);

            var conflict =
                await ShowTimeRepository.ShowTimeConflictExistsAsync(
                    showTime.ScreenId,
                    start,
                    end);

            if (conflict)
            {
                var conflictingShowTime =
                    await ShowTimeRepository.GetShowTimeByIdAsync(showTimeId);

                if (conflictingShowTime.ShowTimeId != showTimeId)
                    throw new InvalidOperationException(
                        MessageStrings.ScreenAlreadyScheduledForSelectedDate);
            }

            showTime.MovieId = updateShowTimeDto.MovieId;
            showTime.LanguageId = updateShowTimeDto.LanguageId;
            showTime.StartTime = start;
            showTime.EndTime = end;
            showTime.BasePrice = updateShowTimeDto.BasePrice;

            await ShowTimeRepository.UpdateShowTimeAsync(showTime);
        }

        /// <inheritdoc/>
        public async Task DeleteShowTimeAsync(Guid showTimeId)
        {
            var showTime =
                await ShowTimeRepository.GetShowTimeByIdAsync(showTimeId);

            await ShowTimeRepository.DeleteShowTimeAsync(showTime);
        }
    }
}