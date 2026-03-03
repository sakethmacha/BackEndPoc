using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Constants;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;

namespace MovieBooking.Application.Services
{
    public class ScreenService : IScreenService
    {
        private readonly IScreenRepository ScreenRepository;

        public ScreenService(IScreenRepository screenRepository)
        {
            ScreenRepository = screenRepository;
        }

        public async Task<List<ScreenResponseDto>> GetScreensAsync()
        {
            var screens = await ScreenRepository.GetScreensAsync();
            return screens.Select(s => new ScreenResponseDto
            {
                ScreenId = s.ScreenId,
                ScreenName = s.ScreenName,
                TheatreId = s.TheatreId,
                SeatLayoutType = s.SeatLayoutType
            }).ToList();
        }

        public async Task<CreateScreenRequest> GetScreenByIdAsync(Guid screenId)
        {
            var screen = await ScreenRepository.GetScreenByIdAsync(screenId);
            var seats = await ScreenRepository.GetScreenSeatsAsync(screenId);

            var seatRows = seats
                .GroupBy(s => s.SeatRow)
                .OrderBy(g => g.Key)
                .Select(g => new CreateSeatRowRequest
                {
                    SeatRow = g.Key,
                    SeatCount = g.Count(),
                    SeatType = g.First().SeatType.ToString(),
                    PriceMultiplier = g.First().PriceMultiplier
                }).ToList();

            return new CreateScreenRequest
            {
                TheatreId = screen.TheatreId,
                ScreenName = screen.ScreenName,
                SeatLayoutType = screen.SeatLayoutType,
                SeatRows = seatRows
            };
        }

        public async Task<List<ScreenResponseDto>> GetScreensByTheatreAsync(Guid theatreId)
        {
            var screens = await ScreenRepository.GetByTheatreIdAsync(theatreId);
            return screens.Select(s => new ScreenResponseDto
            {
                ScreenId = s.ScreenId,
                TheatreId = s.TheatreId,
                ScreenName = s.ScreenName,
                SeatLayoutType = s.SeatLayoutType
            }).ToList();
        }

        public async Task AddScreenAsync(CreateScreenRequest createScreenRequest)
        {
            if (createScreenRequest.SeatRows == null || !createScreenRequest.SeatRows.Any())
                throw new InvalidOperationException(MessageStrings.SeatLayoutIsRequired);

            //if (!Enum.TryParse<SeatLayoutType>(createScreenRequest.SeatLayoutType, true, out var layoutType))
            //    throw new InvalidOperationException(MessageStrings.InvalidSeatLayoutType);

            var seatRows = new List<CreateSeatRowDto>();
            foreach (var row in createScreenRequest.SeatRows)
            {
                if (!Enum.TryParse<SeatType>(row.SeatType, true, out var seatType))
                    throw new InvalidOperationException($"{MessageStrings.InvalidSeatType}: {row.SeatType}");

                seatRows.Add(new CreateSeatRowDto
                {
                    SeatRow = row.SeatRow,
                    SeatCount = row.SeatCount,
                    SeatType = seatType,
                    PriceMultiplier = row.PriceMultiplier
                });
            }

            var dto = new CreateScreenDto
            {
                TheatreId = createScreenRequest.TheatreId,
                ScreenName = createScreenRequest.ScreenName,
                IsActive = true,
                SeatLayoutType = SeatLayoutType.STANDARD,
                SeatRows = seatRows
            };

            await AddScreenInternalAsync(dto);
        }

        private async Task AddScreenInternalAsync(CreateScreenDto createScreenDto)
        {
            if (createScreenDto.SeatRows.Select(r => r.SeatRow).Distinct().Count() != createScreenDto.SeatRows.Count)
                throw new InvalidOperationException(MessageStrings.DuplicateSeatRowsNotAllowed);

            var screen = new Screen
            {
                ScreenId = Guid.NewGuid(),
                TheatreId = createScreenDto.TheatreId,
                ScreenName = createScreenDto.ScreenName,
                SeatLayoutType = createScreenDto.SeatLayoutType,
                IsActive = true
            };

            await ScreenRepository.AddScreenAsync(screen);

            var seats = new List<Seat>();
            foreach (var row in createScreenDto.SeatRows)
            {
                for (int col = 1; col <= row.SeatCount; col++)
                {
                    seats.Add(new Seat
                    {
                        SeatId = Guid.NewGuid(),
                        ScreenId = screen.ScreenId,
                        SeatRow = row.SeatRow,
                        SeatColumn = col,
                        SeatType = row.SeatType,
                        PriceMultiplier = row.PriceMultiplier,
                        IsActive = true
                    });
                }
            }

            await ScreenRepository.AddSeatsAsync(seats);
        }

        public async Task UpdateScreenAsync(Guid screenId, UpdateScreenDto updateScreenDto)
        {
            var screen = await ScreenRepository.GetScreenByIdAsync(screenId);

            var hasActiveShowTimes = await ScreenRepository.ScreenHasActiveShowTimesAsync(screenId);
            if (hasActiveShowTimes)
                throw new InvalidOperationException(MessageStrings.CannotUpdateScreenWithActiveShowTimes);

            if (updateScreenDto.SeatRows == null || !updateScreenDto.SeatRows.Any())
                throw new InvalidOperationException(MessageStrings.SeatLayoutIsRequired);

            if (!Enum.TryParse<SeatLayoutType>(updateScreenDto.SeatLayoutType, true, out var layoutType))
                throw new InvalidOperationException(MessageStrings.InvalidSeatLayoutType);

            var seatRows = new List<CreateSeatRowDto>();
            foreach (var row in updateScreenDto.SeatRows)
            {
                if (!Enum.TryParse<SeatType>(row.SeatType, true, out var seatType))
                    throw new InvalidOperationException($"{MessageStrings.InvalidSeatType}: {row.SeatType}");

                seatRows.Add(new CreateSeatRowDto
                {
                    SeatRow = row.SeatRow,
                    SeatCount = row.SeatCount,
                    SeatType = seatType,
                    PriceMultiplier = row.PriceMultiplier
                });
            }

            if (seatRows.Select(r => r.SeatRow).Distinct().Count() != seatRows.Count)
                throw new InvalidOperationException(MessageStrings.DuplicateSeatRowsNotAllowed);

            screen.ScreenName = updateScreenDto.ScreenName;
            screen.SeatLayoutType = layoutType;

            await ScreenRepository.UpdateScreenAsync(screen);
            await ScreenRepository.DeleteScreenSeatsAsync(screenId);

            var seats = new List<Seat>();
            foreach (var row in seatRows)
            {
                for (int col = 1; col <= row.SeatCount; col++)
                {
                    seats.Add(new Seat
                    {
                        SeatId = Guid.NewGuid(),
                        ScreenId = screen.ScreenId,
                        SeatRow = row.SeatRow,
                        SeatColumn = col,
                        SeatType = row.SeatType,
                        PriceMultiplier = row.PriceMultiplier,
                        IsActive = true
                    });
                }
            }

            await ScreenRepository.AddSeatsAsync(seats);
        }

        public async Task DeleteScreenAsync(Guid screenId)
        {
            var screen = await ScreenRepository.GetScreenByIdAsync(screenId);

            var hasActiveShowTimes = await ScreenRepository.ScreenHasActiveShowTimesAsync(screenId);
            if (hasActiveShowTimes)
                throw new InvalidOperationException(MessageStrings.CannotDeleteScreenWithActiveShowTimes);

            await ScreenRepository.DeleteScreenAsync(screen);
        }
    }
}