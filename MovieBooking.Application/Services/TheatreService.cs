using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;

namespace MovieBooking.Application.Services
{
    /// <summary>
    /// Service implementation for theatre management operations
    /// </summary>
    public class TheatreService : ITheatreService
    {
        private readonly ITheatreRepository TheatreRepository;

        /// <summary>Initializes a new instance of TheatreService</summary>
        public TheatreService(ITheatreRepository theatreRepository)
        {
            TheatreRepository = theatreRepository;
        }

        /// <inheritdoc/>
        public async Task<List<TheatreResponseDto>> GetTheatresAsync()
        {
            var theatres = await TheatreRepository.GetTheatresAsync();
            return theatres.Select(t => new TheatreResponseDto
            {
                TheatreId = t.TheatreId,
                Name = t.Name,
                Location = t.Location,
                IsActive = t.IsActive
            }).ToList();
        }

        /// <inheritdoc/>
        public async Task<TheatreResponseDto> GetTheatreByIdAsync(Guid theatreId)
        {
            var theatre = await TheatreRepository.GetTheatreByIdAsync(theatreId);
            return new TheatreResponseDto
            {
                TheatreId = theatre.TheatreId,
                Name = theatre.Name,
                Location = theatre.Location,
                IsActive = theatre.IsActive,
                TimeSlots = theatre.TimeSlots
                    .OrderBy(ts => ts.StartTime)
                    .Select(ts => new TimeSlotDto
                    {
                        StartTime = ts.StartTime.ToString("HH:mm"),
                        EndTime = ts.EndTime.ToString("HH:mm")
                    }).ToList()
            };
        }

        /// <inheritdoc/>
        public async Task AddTheatreAsync(CreateTheatreDto createTheatreDto, Guid superAdminId)
        {
            if (createTheatreDto.TimeSlots == null || !createTheatreDto.TimeSlots.Any())
                throw new InvalidOperationException("At least one show timing must be configured");

            var parsedSlots = createTheatreDto.TimeSlots.Select(ts =>
            {
                if (!TimeOnly.TryParse(ts.StartTime, out var start))
                    throw new InvalidOperationException($"Invalid start time: {ts.StartTime}");
                if (!TimeOnly.TryParse(ts.EndTime, out var end))
                    throw new InvalidOperationException($"Invalid end time: {ts.EndTime}");
                if (end <= start)
                    throw new InvalidOperationException("End time must be greater than start time");
                return new { Start = start, End = end };
            }).OrderBy(x => x.Start).ToList();

            for (int i = 0; i < parsedSlots.Count - 1; i++)
            {
                if (parsedSlots[i].End > parsedSlots[i + 1].Start)
                    throw new InvalidOperationException("Theatre show timings cannot overlap");
            }

            var theatre = new Theatre
            {
                TheatreId = Guid.NewGuid(),
                Name = createTheatreDto.Name,
                Location = createTheatreDto.Location,
                CreatedBy = superAdminId,
                ApprovalStatus = ApprovalStatus.APPROVED,
                IsActive = true,
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

            await TheatreRepository.AddTheatreWithTimeSlotsAsync(theatre, timeSlots);
        }

        /// <inheritdoc/>
        public async Task UpdateTheatreAsync(Guid theatreId, UpdateTheatreDto updateTheatreDto)
        {
            if (updateTheatreDto.TimeSlots == null || !updateTheatreDto.TimeSlots.Any())
                throw new InvalidOperationException("At least one show timing must be configured");

            var theatre = await TheatreRepository.GetTheatreByIdAsync(theatreId);

            var parsedSlots = updateTheatreDto.TimeSlots.Select(ts =>
            {
                if (!TimeOnly.TryParse(ts.StartTime, out var start))
                    throw new InvalidOperationException($"Invalid start time: {ts.StartTime}");
                if (!TimeOnly.TryParse(ts.EndTime, out var end))
                    throw new InvalidOperationException($"Invalid end time: {ts.EndTime}");
                if (end <= start)
                    throw new InvalidOperationException("End time must be greater than start time");
                return new { Start = start, End = end };
            }).OrderBy(x => x.Start).ToList();

            for (int i = 0; i < parsedSlots.Count - 1; i++)
            {
                if (parsedSlots[i].End > parsedSlots[i + 1].Start)
                    throw new InvalidOperationException("Theatre show timings cannot overlap");
            }

            theatre.Name = updateTheatreDto.Name;
            theatre.Location = updateTheatreDto.Location;

            await TheatreRepository.DeleteTheatreTimeSlotsAsync(theatreId);

            var newTimeSlots = parsedSlots.Select(p => new TheatreTimeSlot
            {
                TheatreTimeSlotId = Guid.NewGuid(),
                TheatreId = theatre.TheatreId,
                StartTime = p.Start,
                EndTime = p.End,
                IsActive = true
            }).ToList();

            await TheatreRepository.AddTheatreWithTimeSlotsAsync(theatre, newTimeSlots);
        }

        /// <inheritdoc/>
        public async Task DeleteTheatreAsync(Guid theatreId)
        {
            var theatre = await TheatreRepository.GetTheatreByIdAsync(theatreId);
            var hasActiveScreens = await TheatreRepository.TheatreHasActiveScreensAsync(theatreId);
            if (hasActiveScreens)
                throw new InvalidOperationException("Cannot delete theatre with active screens. Please deactivate or delete screens first.");
            await TheatreRepository.DeleteTheatreTimeSlotsAsync(theatreId);
            await TheatreRepository.DeleteTheatreAsync(theatre);
        }
    }
}