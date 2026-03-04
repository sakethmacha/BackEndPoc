using MovieBooking.Application.DTOs.Admin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Constants;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;

namespace MovieBooking.Application.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository RequestRepository;

        public RequestService(IRequestRepository requestRepository)
        {
            RequestRepository = requestRepository;
        }


        public async Task<Guid> RequestTheatreAsync(
            CreateTheatreRequestDto createtheatreRequestDto,
            Guid adminId)
        {
            if (createtheatreRequestDto.TimeSlots == null ||
                !createtheatreRequestDto.TimeSlots.Any())
                throw new InvalidOperationException(
                    MessageStrings.AtLeastOneShowTimingRequired);

            var parsedSlots = createtheatreRequestDto.TimeSlots.Select(ts =>
            {
                if (!TimeOnly.TryParse(ts.StartTime, out var start))
                    throw new InvalidOperationException(
                        $"{MessageStrings.InvalidStartTime}: {ts.StartTime}");

                if (!TimeOnly.TryParse(ts.EndTime, out var end))
                    throw new InvalidOperationException(
                        $"{MessageStrings.InvalidEndTime}: {ts.EndTime}");

                if (end <= start)
                    throw new InvalidOperationException(
                        MessageStrings.EndTimeMustBeGreaterThanStartTime);

                return new { Start = start, End = end };
            })
            .OrderBy(x => x.Start)
            .ToList();

            for (int i = 0; i < parsedSlots.Count - 1; i++)
            {
                if (parsedSlots[i].End > parsedSlots[i + 1].Start)
                    throw new InvalidOperationException(
                        MessageStrings.TheatreShowTimingsCannotOverlap);
            }

            var theatre = new Theatre
            {
                TheatreId = Guid.NewGuid(),
                Name = createtheatreRequestDto.Name,
                Location = createtheatreRequestDto.Location,
                CreatedBy = adminId,
                ApprovalStatus = ApprovalStatus.PENDING,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };

            var timeSlots = parsedSlots.Select(p => new TheatreTimeSlot
            {
                TheatreTimeSlotId = Guid.NewGuid(),
                TheatreId = theatre.TheatreId,
                StartTime = p.Start,
                EndTime = p.End,
                IsActive = true
            }).ToList();

            var request = new AdminRequest
            {
                AdminRequestId = Guid.NewGuid(),
                RequestedBy = adminId,
                RequestType = RequestType.THEATRE,
                Status = ApprovalStatus.PENDING,
                RequestedAt = DateTime.UtcNow
            };

            return await RequestRepository.CreateTheatreRequestAsync(
                theatre, timeSlots, request);
        }

        public async Task<List<TheatreRequestResponseDto>>
            GetTheatreRequestsAsync(Guid adminId)
        {
            var theatres = await RequestRepository.GetTheatresByAdminAsync(adminId);

            return theatres.Select(t => new TheatreRequestResponseDto
            {
                TheatreId = t.TheatreId,
                Name = t.Name,
                Location = t.Location,
                ApprovalStatus = t.ApprovalStatus.ToString(),
                RequestedAt = t.CreatedAt,
                TimeSlots = t.TimeSlots.Select(ts => new TimeSlotResponseDto
                {
                    StartTime = ts.StartTime.ToString("HH:mm"),
                    EndTime = ts.EndTime.ToString("HH:mm")
                }).ToList()
            }).ToList();
        }

        public async Task<List<TheatreRequestResponseDto>>
            GetApprovedTheatresAsync(Guid adminId)
        {
            var theatres = await RequestRepository.GetTheatresByAdminAsync(adminId);

            return theatres
                .Where(t => t.ApprovalStatus == ApprovalStatus.APPROVED)
                .Select(t => new TheatreRequestResponseDto
                {
                    TheatreId = t.TheatreId,
                    Name = t.Name,
                    Location = t.Location,
                    ApprovalStatus = t.ApprovalStatus.ToString(),
                    RequestedAt = t.CreatedAt,
                    TimeSlots = t.TimeSlots.Select(ts => new TimeSlotResponseDto
                    {
                        StartTime = ts.StartTime.ToString("HH:mm"),
                        EndTime = ts.EndTime.ToString("HH:mm")
                    }).ToList()
                }).ToList();
        }

        public async Task<Guid> RequestScreenAsync(
            CreateScreenRequestDto createScreenRequestDto,
            Guid adminId)
        {
            var theatre = await RequestRepository
                .GetTheatreByIdAsync(createScreenRequestDto.TheatreId);

            if (theatre.CreatedBy != adminId)
                throw new UnauthorizedAccessException(
                    MessageStrings.CanOnlyAddScreensToOwnTheatres);

            if (theatre.ApprovalStatus != ApprovalStatus.APPROVED)
                throw new InvalidOperationException(
                    MessageStrings.CannotAddScreenToUnapprovedTheatre);

            if (createScreenRequestDto.SeatRows == null ||
                !createScreenRequestDto.SeatRows.Any())
                throw new InvalidOperationException(
                    MessageStrings.SeatLayoutIsRequired);

            if (!Enum.TryParse<SeatLayoutType>(
                    createScreenRequestDto.SeatLayoutType,
                    true,
                    out var layoutType))
                throw new InvalidOperationException(
                    MessageStrings.InvalidSeatLayoutType);

            if (createScreenRequestDto.SeatRows
                    .Select(r => r.SeatRow)
                    .Distinct()
                    .Count() != createScreenRequestDto.SeatRows.Count)
                throw new InvalidOperationException(
                    MessageStrings.DuplicateSeatRowsNotAllowed);

            var screen = new Screen
            {
                ScreenId = Guid.NewGuid(),
                TheatreId = createScreenRequestDto.TheatreId,
                ScreenName = createScreenRequestDto.ScreenName,
                SeatLayoutType = layoutType,
                ApprovalStatus = ApprovalStatus.PENDING,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };

            var seats = new List<Seat>();

            foreach (var row in createScreenRequestDto.SeatRows)
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
                        IsActive = false
                    });
                }
            }

            var request = new AdminRequest
            {
                AdminRequestId = Guid.NewGuid(),
                RequestedBy = adminId,
                RequestType = RequestType.SCREEN,
                Status = ApprovalStatus.PENDING,
                RequestedAt = DateTime.UtcNow
            };

            return await RequestRepository
                .CreateScreenRequestAsync(screen, seats, request);
        }

        public async Task<List<ScreenRequestResponseDto>>
            GetScreenRequestsAsync(Guid adminId)
        {
            var screens = await RequestRepository.GetScreensByAdminAsync(adminId);

            return screens.Select(s => new ScreenRequestResponseDto
            {
                ScreenId = s.ScreenId,
                ScreenName = s.ScreenName,
                TheatreName = s.Theatre.Name,
                SeatLayoutType = s.SeatLayoutType.ToString(),
                ApprovalStatus = s.ApprovalStatus.ToString(),
                RequestedAt = s.CreatedAt
            }).ToList();
        }

        public async Task<List<ScreenRequestResponseDto>>
            GetApprovedScreensAsync(Guid adminId)
        {
            var screens = await RequestRepository.GetScreensByAdminAsync(adminId);

            return screens
                .Where(s => s.ApprovalStatus == ApprovalStatus.APPROVED)
                .Select(s => new ScreenRequestResponseDto
                {
                    ScreenId = s.ScreenId,
                    ScreenName = s.ScreenName,
                    TheatreName = s.Theatre.Name,
                    SeatLayoutType = s.SeatLayoutType.ToString(),
                    ApprovalStatus = s.ApprovalStatus.ToString(),
                    RequestedAt = s.CreatedAt
                }).ToList();
        }

        public async Task<List<TheatreRequestResponseDto>>
            GetTheatresForScreenAsync(Guid adminId)
        {
            var theatres = await RequestRepository.GetTheatresByAdminAsync(adminId);

            return theatres
                .Where(t => t.ApprovalStatus == ApprovalStatus.APPROVED && t.IsActive)
                .Select(t => new TheatreRequestResponseDto
                {
                    TheatreId = t.TheatreId,
                    Name = t.Name,
                    Location = t.Location,
                    ApprovalStatus = t.ApprovalStatus.ToString(),
                    RequestedAt = t.CreatedAt
                }).ToList();
        }
    }
}