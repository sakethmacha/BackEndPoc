using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;

namespace MovieBooking.Application.Services
{
    public class SuperAdminService : ISuperAdminService
    {
        private readonly ISuperAdminRepository _repo;

        public SuperAdminService(ISuperAdminRepository repo)
        {
            _repo = repo;
        }

        public async Task CreateAdminAsync(CreateAdminDto dto)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.CreateAdminAsync(user);
        }

        public Task<List<AdminDto>> GetAdminsAsync()
            => _repo.GetAdminsAsync();

        public async Task ToggleAdminAsync(Guid adminId)
        {
            var admin = await _repo.GetUserByIdAsync(adminId);
            admin.IsActive = !admin.IsActive;
            await _repo.UpdateUserAsync(admin);
        }

        public async Task AddMovieAsync(AddMovieDto dto)
        {
            var movie = new Movie
            {
                MovieId = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                DurationMinutes = dto.DurationMinutes,
                ReleaseDate = dto.ReleaseDate,
                PosterUrl = dto.PosterUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddMovieAsync(movie);
        }

        public async Task<List<MovieResponse>> GetMoviesAsync()
        {
            var movies = await _repo.GetAllAsync();

            return movies.Select(m => new MovieResponse
            {
                MovieId = m.MovieId,
                Title = m.Title,
                DurationMinutes = m.DurationMinutes,
                ReleaseDate = m.ReleaseDate,
                IsActive = m.IsActive
            }).ToList();
        }

        public async Task ToggleMovieAsync(Guid movieId)
        {
            var movie = await _repo.GetMovieByIdAsync(movieId);
            movie.IsActive = !movie.IsActive;
            await _repo.UpdateMovieAsync(movie);
        }
        public async Task AddTheatreAsync(CreateTheatreDto dto, Guid superAdminId)
        {
            if (dto.TimeSlots == null || !dto.TimeSlots.Any())
                throw new InvalidOperationException(
                    "At least one show timing must be configured");

            // 1️⃣ Parse + normalize times
            var parsedSlots = dto.TimeSlots.Select(ts =>
            {
                if (!TimeOnly.TryParse(ts.StartTime, out var start))
                    throw new InvalidOperationException(
                        $"Invalid start time: {ts.StartTime}");

                if (!TimeOnly.TryParse(ts.EndTime, out var end))
                    throw new InvalidOperationException(
                        $"Invalid end time: {ts.EndTime}");

                if (end <= start)
                    throw new InvalidOperationException(
                        "End time must be greater than start time");

                return new
                {
                    Start = start,
                    End = end
                };
            })
            .OrderBy(x => x.Start)
            .ToList();

            // 2️⃣ Check overlapping slots
            for (int i = 0; i < parsedSlots.Count - 1; i++)
            {
                if (parsedSlots[i].End > parsedSlots[i + 1].Start)
                    throw new InvalidOperationException(
                        "Theatre show timings cannot overlap");
            }

            // 3️⃣ Create theatre
            var theatre = new Theatre
            {
                TheatreId = Guid.NewGuid(),
                Name = dto.Name,
                Location = dto.Location,
                CreatedBy = superAdminId,
                ApprovalStatus = ApprovalStatus.APPROVED,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // 4️⃣ Create TheatreTimeSlot entities
            var timeSlots = parsedSlots.Select(p => new TheatreTimeSlot
            {
                TheatreTimeSlotId = Guid.NewGuid(),
                TheatreId = theatre.TheatreId,
                StartTime = p.Start,
                EndTime = p.End,
                IsActive = true
            }).ToList();

            // 5️⃣ Persist
            await _repo.AddTheatreWithTimeSlotsAsync(theatre, timeSlots);
        }


        // This method is called by the Controller
        public async Task AddScreenAsync(CreateScreenRequest request)
        {
            if (request.SeatRows == null || !request.SeatRows.Any())
                throw new InvalidOperationException("Seat layout is required");

            // ✅ parse SeatLayoutType (string → enum)
            if (!Enum.TryParse<SeatLayoutType>(
                    request.SeatLayoutType, true, out var layoutType))
                throw new InvalidOperationException("Invalid seat layout type");

            var seatRows = new List<CreateSeatRowDto>();

            foreach (var row in request.SeatRows)
            {
                // ✅ parse SeatType (string → enum)
                if (!Enum.TryParse<SeatType>(
                        row.SeatType, true, out var seatType))
                    throw new InvalidOperationException(
                        $"Invalid seat type: {row.SeatType}");

                seatRows.Add(new CreateSeatRowDto
                {
                    SeatRow = row.SeatRow,
                    SeatCount = row.SeatCount,
                    SeatType = seatType,              // enum stored
                    PriceMultiplier = row.PriceMultiplier
                });
            }

            // build application DTO (ENUMS ONLY)
            var dto = new CreateScreenDto
            {
                TheatreId = request.TheatreId,
                ScreenName = request.ScreenName,
                SeatLayoutType = layoutType,          // enum
                SeatRows = seatRows
            };

            await AddScreenInternalAsync(dto);
        }
        private async Task AddScreenInternalAsync(CreateScreenDto dto)
{
    // 🔐 validations (enum-safe)
    if (dto.SeatRows.Select(r => r.SeatRow).Distinct().Count()
        != dto.SeatRows.Count)
        throw new InvalidOperationException("Duplicate seat rows are not allowed");

    var screen = new Screen
    {
        ScreenId = Guid.NewGuid(),
        TheatreId = dto.TheatreId,
        ScreenName = dto.ScreenName,
        SeatLayoutType = dto.SeatLayoutType, // enum ✅
        IsActive = true
    };

    await _repo.AddScreenAsync(screen);

    var seats = new List<Seat>();

    foreach (var row in dto.SeatRows)
    {
        for (int col = 1; col <= row.SeatCount; col++)
        {
            seats.Add(new Seat
            {
                SeatId = Guid.NewGuid(),
                ScreenId = screen.ScreenId,
                SeatRow = row.SeatRow,
                SeatColumn = col,
                SeatType = row.SeatType,       // enum ✅
                PriceMultiplier = row.PriceMultiplier,
                IsActive = true
            });
        }
    }

    await _repo.AddSeatsAsync(seats);
}



        public async Task AddShowTimeAsync(CreateShowTimeDto dto)
        {
            // 1️⃣ Get theatre timings
            var slots = await _repo.GetTimeSlotsByTheatreAsync(dto.TheatreId);

            if (!slots.Any())
                throw new InvalidOperationException(
                    "Theatre has no configured time slots");

            var showTimes = new List<ShowTime>();

            foreach (var slot in slots)
            {
                var start = dto.ShowDate.ToDateTime(slot.StartTime);
                var end = dto.ShowDate.ToDateTime(slot.EndTime);

               // 2️ Business rule: no conflict per screen
                bool conflict = await _repo.ShowTimeConflictExistsAsync(
                    dto.ScreenId, start, end);

                if (conflict)
                    throw new InvalidOperationException(
                        "This screen is already scheduled for the selected date");

                showTimes.Add(new ShowTime
                {
                    ShowTimeId = Guid.NewGuid(),
                    TheatreId = dto.TheatreId,
                    ScreenId = dto.ScreenId,
                    MovieId = dto.MovieId,
                    LanguageId = dto.LanguageId,
                    StartTime = start,
                    EndTime = end,
                    BasePrice = dto.BasePrice,
                    IsActive = true
                });
            }

            // 3️⃣ Persist
            await _repo.AddShowTimesAsync(showTimes);
        }


        public async Task ApproveRequestAsync(Guid requestId)
        {
            var request = await _repo.GetRequestByIdAsync(requestId);
            request.Status = ApprovalStatus.APPROVED;
            request.ReviewedAt = DateTime.UtcNow;

            switch (request.RequestType)
            {
                case RequestType.THEATRE:
                    await _repo.ApproveTheatreAsync(request.ReferenceId);
                    break;
                case RequestType.SCREEN:
                    await _repo.ApproveScreenAsync(request.ReferenceId);
                    break;
                case RequestType.SHOWTIME:
                    await _repo.ApproveShowTimeAsync(request.ReferenceId);
                    break;
            }

            await _repo.UpdateRequestAsync(request);
        }

        public async Task RejectRequestAsync(Guid requestId)
        {
            var request = await _repo.GetRequestByIdAsync(requestId);
            request.Status = ApprovalStatus.REJECTED;
            request.ReviewedAt = DateTime.UtcNow;
            await _repo.UpdateRequestAsync(request);
        }

        public async Task AddLanguageAsync(CreateLanguageDto dto)
        {
            var exists = await _repo.LanguageExistsAsync(dto.Name);
            if (exists)
                throw new InvalidOperationException("Language already exists");

            var language = new Language
            {
                LanguageId = Guid.NewGuid(),
                Name = dto.Name.Trim()
            };

            await _repo.AddLanguageAsync(language);
        }

        public async Task<List<LanguageDto>> GetLanguagesAsync()
        {
            var languages = await _repo.GetLanguagesAsync();
            return languages.Select(l => new LanguageDto
            {
                LanguageId = l.LanguageId,
                Name = l.Name
            }).ToList();
        }
        public async Task<List<TheatreResponseDto>> GetTheatresAsync()
        {
            var theatres = await _repo.GetTheatresAsync();

            return theatres.Select(t => new TheatreResponseDto
            {
                TheatreId = t.TheatreId,
                Name = t.Name,
                Location = t.Location,
                IsActive = t.IsActive
            }).ToList();
        }
        // ---------- SCREENS ----------
        public async Task<List<ScreenResponseDto>> GetScreensAsync()
        {
            var screens = await _repo.GetScreensAsync();

            return screens.Select(s => new ScreenResponseDto
            {
                ScreenId = s.ScreenId,
                ScreenName = s.ScreenName,
                TheatreId = s.TheatreId,
                SeatLayoutType = s.SeatLayoutType
            }).ToList();
        }

        // ---------- SHOWTIMES ----------
        public async Task<List<ShowTimeResponseDto>> GetShowTimesAsync()
        {
            var showTimes = await _repo.GetShowTimesAsync();

            return showTimes.Select(st => new ShowTimeResponseDto
            {
                ShowTimeId = st.ShowTimeId,

                MovieTitle = st.Movie.Title,
                TheatreName = st.Theatre.Name,
                ScreenName = st.Screen.ScreenName,
                LanguageName = st.Language.Name,

                StartTime = st.StartTime,
                EndTime = st.EndTime,
                BasePrice = st.BasePrice
            }).ToList();
        }

        public async Task<List<ScreenResponseDto>> GetScreensByTheatreAsync(Guid theatreId)
        {
            var screens = await _repo.GetByTheatreIdAsync(theatreId);

            return screens.Select(s => new ScreenResponseDto
            {
                ScreenId = s.ScreenId,
                TheatreId = s.TheatreId,
                ScreenName = s.ScreenName,
                SeatLayoutType = s.SeatLayoutType
            }).ToList();
        }
        // ========== UPDATE METHODS ==========

        public async Task UpdateMovieAsync(Guid movieId, UpdateMovieDto dto)
        {
            var movie = await _repo.GetMovieByIdAsync(movieId);

            movie.Title = dto.Title;
            movie.Description = dto.Description;
            movie.DurationMinutes = dto.DurationMinutes;
            movie.ReleaseDate = dto.ReleaseDate;
            movie.PosterUrl = dto.PosterUrl;

            await _repo.UpdateMovieAsync(movie);
        }

        public async Task UpdateTheatreAsync(Guid theatreId, UpdateTheatreDto dto)
        {
            if (dto.TimeSlots == null || !dto.TimeSlots.Any())
                throw new InvalidOperationException("At least one show timing must be configured");

            var theatre = await _repo.GetTheatreByIdAsync(theatreId);

            // Validate and parse time slots
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

            // Update theatre details
            theatre.Name = dto.Name;
            theatre.Location = dto.Location;

            // Delete existing time slots
            await _repo.DeleteTheatreTimeSlotsAsync(theatreId);

            // Create new time slots
            var newTimeSlots = parsedSlots.Select(p => new Domain.Entities.TheatreTimeSlot
            {
                TheatreTimeSlotId = Guid.NewGuid(),
                TheatreId = theatre.TheatreId,
                StartTime = p.Start,
                EndTime = p.End,
                IsActive = true
            }).ToList();

            await _repo.AddTheatreWithTimeSlotsAsync(theatre, newTimeSlots);
        }

        public async Task UpdateScreenAsync(Guid screenId, UpdateScreenDto dto)
        {
            var screen = await _repo.GetScreenByIdAsync(screenId);

            // Check if screen has active showtimes
            var hasActiveShowTimes = await _repo.ScreenHasActiveShowTimesAsync(screenId);
            if (hasActiveShowTimes)
                throw new InvalidOperationException("Cannot update screen with active showtimes. Please deactivate or delete showtimes first.");

            if (dto.SeatRows == null || !dto.SeatRows.Any())
                throw new InvalidOperationException("Seat layout is required");

            // Parse SeatLayoutType
            if (!Enum.TryParse<SeatLayoutType>(dto.SeatLayoutType, true, out var layoutType))
                throw new InvalidOperationException("Invalid seat layout type");

            var seatRows = new List<CreateSeatRowDto>();

            foreach (var row in dto.SeatRows)
            {
                if (!Enum.TryParse<SeatType>(row.SeatType, true, out var seatType))
                    throw new InvalidOperationException($"Invalid seat type: {row.SeatType}");

                seatRows.Add(new CreateSeatRowDto
                {
                    SeatRow = row.SeatRow,
                    SeatCount = row.SeatCount,
                    SeatType = seatType,
                    PriceMultiplier = row.PriceMultiplier
                });
            }

            // Validate duplicate rows
            if (seatRows.Select(r => r.SeatRow).Distinct().Count() != seatRows.Count)
                throw new InvalidOperationException("Duplicate seat rows are not allowed");

            // Update screen
            screen.ScreenName = dto.ScreenName;
            screen.SeatLayoutType = layoutType;

            await _repo.UpdateScreenAsync(screen);

            // Delete existing seats
            await _repo.DeleteScreenSeatsAsync(screenId);

            // Create new seats
            var seats = new List<Domain.Entities.Seat>();

            foreach (var row in seatRows)
            {
                for (int col = 1; col <= row.SeatCount; col++)
                {
                    seats.Add(new Domain.Entities.Seat
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

            await _repo.AddSeatsAsync(seats);
        }

        public async Task UpdateShowTimeAsync(Guid showTimeId, UpdateShowTimeDto dto)
        {
            var showTime = await _repo.GetShowTimeByIdAsync(showTimeId);

            // Get theatre time slots
            var slots = await _repo.GetTimeSlotsByTheatreAsync(showTime.TheatreId);

            if (!slots.Any())
                throw new InvalidOperationException("Theatre has no configured time slots");

            // Use the first slot (or you can modify logic to allow selecting a specific slot)
            var slot = slots.First();

            var start = dto.ShowDate.ToDateTime(slot.StartTime);
            var end = dto.ShowDate.ToDateTime(slot.EndTime);

            // Check for conflicts (excluding current showtime)
            var conflict = await _repo.ShowTimeConflictExistsAsync(showTime.ScreenId, start, end);

            if (conflict)
            {
                // Additional check: ensure it's not conflicting with itself
                var conflictingShowTime = await _repo.GetShowTimeByIdAsync(showTimeId);
                if (conflictingShowTime.ShowTimeId != showTimeId)
                    throw new InvalidOperationException("This screen is already scheduled for the selected date");
            }

            // Update showtime
            showTime.MovieId = dto.MovieId;
            showTime.LanguageId = dto.LanguageId;
            showTime.StartTime = start;
            showTime.EndTime = end;
            showTime.BasePrice = dto.BasePrice;

            await _repo.UpdateShowTimeAsync(showTime);
        }

        public async Task UpdateLanguageAsync(Guid languageId, UpdateLanguageDto dto)
        {
            var language = await _repo.GetLanguageByIdAsync(languageId);

            // Check if name already exists (excluding current language)
            var exists = await _repo.LanguageExistsAsync(dto.Name);
            if (exists && !language.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Language name already exists");

            language.Name = dto.Name.Trim();

            await _repo.UpdateLanguageAsync(language);
        }

        public async Task UpdateAdminAsync(Guid adminId, UpdateAdminDto dto)
        {
            var admin = await _repo.GetUserByIdAsync(adminId);

            if (admin.Role != UserRole.Admin)
                throw new InvalidOperationException("User is not an admin");

            admin.Name = dto.Name;
            admin.Email = dto.Email;

            // Update password only if provided
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                admin.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _repo.UpdateUserAsync(admin);
        }

        // ========== DELETE METHODS ==========

        public async Task DeleteMovieAsync(Guid movieId)
        {
            var movie = await _repo.GetMovieByIdAsync(movieId);

            // Check if movie has active showtimes
            var hasActiveShowTimes = await _repo.MovieHasActiveShowTimesAsync(movieId);

            if (hasActiveShowTimes)
                throw new InvalidOperationException("Cannot delete movie with active showtimes. Please deactivate or delete showtimes first.");

            await _repo.DeleteMovieAsync(movie);
        }

        public async Task DeleteTheatreAsync(Guid theatreId)
        {
            var theatre = await _repo.GetTheatreByIdAsync(theatreId);

            // Check if theatre has active screens
            var hasActiveScreens = await _repo.TheatreHasActiveScreensAsync(theatreId);

            if (hasActiveScreens)
                throw new InvalidOperationException("Cannot delete theatre with active screens. Please deactivate or delete screens first.");

            // Delete time slots first (cascade)
            await _repo.DeleteTheatreTimeSlotsAsync(theatreId);

            await _repo.DeleteTheatreAsync(theatre);
        }

        public async Task DeleteScreenAsync(Guid screenId)
        {
            var screen = await _repo.GetScreenByIdAsync(screenId);

            // Check if screen has active showtimes
            var hasActiveShowTimes = await _repo.ScreenHasActiveShowTimesAsync(screenId);

            if (hasActiveShowTimes)
                throw new InvalidOperationException("Cannot delete screen with active showtimes. Please deactivate or delete showtimes first.");

            // Delete seats first (cascade)
            await _repo.DeleteScreenSeatsAsync(screenId);

            await _repo.DeleteScreenAsync(screen);
        }

        public async Task DeleteShowTimeAsync(Guid showTimeId)
        {
            var showTime = await _repo.GetShowTimeByIdAsync(showTimeId);

            // You can add additional checks here (e.g., bookings exist)
            // For now, simple delete
            await _repo.DeleteShowTimeAsync(showTime);
        }

        public async Task DeleteLanguageAsync(Guid languageId)
        {
            var language = await _repo.GetLanguageByIdAsync(languageId);

            // Check if language has active showtimes
            var hasActiveShowTimes = await _repo.LanguageHasActiveShowTimesAsync(languageId);

            if (hasActiveShowTimes)
                throw new InvalidOperationException("Cannot delete language with active showtimes. Please deactivate or delete showtimes first.");

            await _repo.DeleteLanguageAsync(language);
        }

        public async Task DeleteAdminAsync(Guid adminId)
        {
            var admin = await _repo.GetUserByIdAsync(adminId);

            if (admin.Role != UserRole.Admin)
                throw new InvalidOperationException("User is not an admin");

            // Check if admin has active theatres
            var hasActiveTheatres = await _repo.AdminHasActiveTheatresAsync(adminId);

            if (hasActiveTheatres)
                throw new InvalidOperationException("Cannot delete admin with active theatres. Please reassign or delete theatres first.");

            await _repo.DeleteAdminAsync(admin);
        }


        public async Task<MovieResponse> GetMovieByIdAsync(Guid movieId)
        {
            var movie = await _repo.GetMovieByIdAsync(movieId);

            return new MovieResponse
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                DurationMinutes = movie.DurationMinutes,
                ReleaseDate = movie.ReleaseDate,
                IsActive = movie.IsActive
            };
        }

        // ⭐ Theatre Details with TimeSlots
        public async Task<TheatreResponseDto> GetTheatreByIdAsync(Guid theatreId)
        {
            var theatre = await _repo.GetTheatreByIdAsync(theatreId);

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
                    })
                    .ToList()
            };
        }

        // ⭐ Screen Details with Seat Layout
        public async Task<CreateScreenRequest> GetScreenByIdAsync(Guid screenId)
        {
            var screen = await _repo.GetScreenByIdAsync(screenId);
            var seats = await _repo.GetScreenSeatsAsync(screenId);

            // Group seats by row to reconstruct the seat layout
            var seatRows = seats
                .GroupBy(s => s.SeatRow)
                .OrderBy(g => g.Key)
                .Select(g => new CreateSeatRowRequest
                {
                    SeatRow = g.Key,
                    SeatCount = g.Count(),
                    SeatType = g.First().SeatType.ToString(),
                    PriceMultiplier = g.First().PriceMultiplier
                })
                .ToList();

            return new CreateScreenRequest
            {
                TheatreId = screen.TheatreId,
                ScreenName = screen.ScreenName,
                SeatLayoutType = screen.SeatLayoutType.ToString(),
                SeatRows = seatRows
            };
        }

        // ⭐ ShowTime Details
        public async Task<ShowTimeResponseDto> GetShowTimeByIdAsync(Guid showTimeId)
        {
            var showTime = await _repo.GetShowTimeByIdAsync(showTimeId);

            return new ShowTimeResponseDto
            {
                ShowTimeId = showTime.ShowTimeId,
                MovieTitle = showTime.Movie.Title,
                TheatreName = showTime.Theatre.Name,
                ScreenName = showTime.Screen.ScreenName,
                LanguageName = showTime.Language.Name,
                StartTime = showTime.StartTime,
                EndTime = showTime.EndTime,
                BasePrice = showTime.BasePrice,
            };
        }

        // ⭐ Language Details (simple)
        public async Task<LanguageDto> GetLanguageByIdAsync(Guid languageId)
        {
            var language = await _repo.GetLanguageByIdAsync(languageId);

            return new LanguageDto
            {
                LanguageId = language.LanguageId,
                Name = language.Name
            };
        }

        // ⭐ Admin Details
        public async Task<AdminDto> GetAdminByIdAsync(Guid adminId)
        {
            var admin = await _repo.GetUserByIdAsync(adminId);

            if (admin.Role != UserRole.Admin)
                throw new InvalidOperationException("User is not an admin");

            return new AdminDto
            {
                UserId = admin.UserId,
                Name = admin.Name,
                Email = admin.Email,
                IsActive = admin.IsActive
            };
        }
    }
}
