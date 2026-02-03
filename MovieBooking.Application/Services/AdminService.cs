using MovieBooking.Application.DTOs.Admin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;

namespace MovieBooking.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repo;

        public AdminService(IAdminRepository repo)
        {
            _repo = repo;
        }

        // ========== THEATRE REQUESTS ==========

        public async Task<Guid> RequestTheatreAsync(CreateTheatreRequestDto dto, Guid adminId)
        {
            // Validate time slots
            if (dto.TimeSlots == null || !dto.TimeSlots.Any())
                throw new InvalidOperationException("At least one show timing must be configured");

            // Parse and validate time slots
            var parsedSlots = dto.TimeSlots.Select(ts =>
            {
                if (!TimeOnly.TryParse(ts.StartTime, out var start))
                    throw new InvalidOperationException($"Invalid start time: {ts.StartTime}");

                if (!TimeOnly.TryParse(ts.EndTime, out var end))
                    throw new InvalidOperationException($"Invalid end time: {ts.EndTime}");

                if (end <= start)
                    throw new InvalidOperationException("End time must be greater than start time");

                return new { Start = start, End = end };
            })
            .OrderBy(x => x.Start)
            .ToList();

            // Check for overlapping slots
            for (int i = 0; i < parsedSlots.Count - 1; i++)
            {
                if (parsedSlots[i].End > parsedSlots[i + 1].Start)
                    throw new InvalidOperationException("Theatre show timings cannot overlap");
            }

            // Create theatre entity
            var theatre = new Theatre
            {
                TheatreId = Guid.NewGuid(),
                Name = dto.Name,
                Location = dto.Location,
                CreatedBy = adminId,
                ApprovalStatus = ApprovalStatus.PENDING,
                IsActive = false, // Will be activated upon approval
                CreatedAt = DateTime.UtcNow
            };

            // Create time slots
            var timeSlots = parsedSlots.Select(p => new TheatreTimeSlot
            {
                TheatreTimeSlotId = Guid.NewGuid(),
                TheatreId = theatre.TheatreId,
                StartTime = p.Start,
                EndTime = p.End,
                IsActive = true 
            }).ToList();

            // Create admin request
            var request = new AdminRequest
            {
                AdminRequestId = Guid.NewGuid(),
                RequestedBy = adminId,
                RequestType = RequestType.THEATRE,
                Status = ApprovalStatus.PENDING,
                RequestedAt = DateTime.UtcNow
            };

            // Save to database
            return await _repo.CreateTheatreRequestAsync(theatre, timeSlots, request);
        }

        public async Task<List<TheatreRequestResponseDto>> GetMyTheatreRequestsAsync(Guid adminId)
        {
            var theatres = await _repo.GetTheatresByAdminAsync(adminId);

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

        public async Task<List<TheatreRequestResponseDto>> GetMyApprovedTheatresAsync(Guid adminId)
        {
            var theatres = await _repo.GetTheatresByAdminAsync(adminId);

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

        // ========== SCREEN REQUESTS ==========


        public async Task<Guid> RequestScreenAsync(CreateScreenRequestDto dto, Guid adminId)
        {
            // Verify the theatre belongs to this admin and is approved
            var theatre = await _repo.GetTheatreByIdAsync(dto.TheatreId);

            if (theatre.CreatedBy != adminId)
                throw new UnauthorizedAccessException("You can only add screens to your own theatres");

            if (theatre.ApprovalStatus != ApprovalStatus.APPROVED)
                throw new InvalidOperationException("Cannot add screen to a theatre that is not approved");

            // Validate seat rows
            if (dto.SeatRows == null || !dto.SeatRows.Any())
                throw new InvalidOperationException("Seat layout is required");

            // Parse SeatLayoutType
            if (!Enum.TryParse<SeatLayoutType>(dto.SeatLayoutType, true, out var layoutType))
                throw new InvalidOperationException("Invalid seat layout type");

            // Check for duplicate rows
            if (dto.SeatRows.Select(r => r.SeatRow).Distinct().Count() != dto.SeatRows.Count)
                throw new InvalidOperationException("Duplicate seat rows are not allowed");

            // Create screen entity
            var screen = new Screen
            {
                ScreenId = Guid.NewGuid(),
                TheatreId = dto.TheatreId,
                ScreenName = dto.ScreenName,
                SeatLayoutType = layoutType,
                ApprovalStatus = ApprovalStatus.PENDING,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };

            // Create seats
            var seats = new List<Seat>();
            foreach (var row in dto.SeatRows)
            {
                var seatTypeEnum = row.SeatType;

                for (int col = 1; col <= row.SeatCount; col++)
                {
                    seats.Add(new Seat
                    {
                        SeatId = Guid.NewGuid(),
                        ScreenId = screen.ScreenId,
                        SeatRow = row.SeatRow,
                        SeatColumn = col,
                        SeatType = seatTypeEnum,
                        PriceMultiplier = row.PriceMultiplier,
                        IsActive = false
                    });
                }
            }

            // Create admin request
            var request = new AdminRequest
            {
                AdminRequestId = Guid.NewGuid(),
                RequestedBy = adminId,
                RequestType = RequestType.SCREEN,
                Status = ApprovalStatus.PENDING,
                RequestedAt = DateTime.UtcNow
            };

            // Save to database
            return await _repo.CreateScreenRequestAsync(screen, seats, request);
        }

        public async Task<List<ScreenRequestResponseDto>> GetMyScreenRequestsAsync(Guid adminId)
        {
            var screens = await _repo.GetScreensByAdminAsync(adminId);

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

        public async Task<List<ScreenRequestResponseDto>> GetMyApprovedScreensAsync(Guid adminId)
        {
            var screens = await _repo.GetScreensByAdminAsync(adminId);

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

        public async Task<List<TheatreRequestResponseDto>> GetMyTheatresForScreenAsync(Guid adminId)
        {
            var theatres = await _repo.GetTheatresByAdminAsync(adminId);

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