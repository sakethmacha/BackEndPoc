using MovieBooking.Application.DTOs.Admin;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;

namespace MovieBooking.Application.Services
{
    public class SuperAdminService : ISuperAdminService
    {
        private readonly ISuperAdminRepository SuperAdminRepository;

        public SuperAdminService(ISuperAdminRepository superAdminRepository)
        {
            SuperAdminRepository = superAdminRepository;
        }

        public async Task CreateAdminAsync(CreateAdminDto createAdminDto)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Name = createAdminDto.Name,
                Email = createAdminDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(createAdminDto.Password),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await SuperAdminRepository.CreateAdminAsync(user);
        }

        public Task<List<AdminDto>> GetAdminsAsync()
            => SuperAdminRepository.GetAdminsAsync();

        public async Task ToggleAdminAsync(Guid adminId)
        {
            var admin = await SuperAdminRepository.GetUserByIdAsync(adminId);
            admin.IsActive = !admin.IsActive;
            await SuperAdminRepository.UpdateUserAsync(admin);
        }

        public async Task AddMovieAsync(AddMovieDto addMovieDto)
        {
            var movie = new Movie
            {
                MovieId = Guid.NewGuid(),
                Title = addMovieDto.Title,
                Description = addMovieDto.Description,
                DurationMinutes = addMovieDto.DurationMinutes,
                ReleaseDate = addMovieDto.ReleaseDate,
                PosterUrl = addMovieDto.PosterUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await SuperAdminRepository.AddMovieAsync(movie);
        }

        public async Task<List<MovieResponse>> GetMoviesAsync()
        {
            var movies = await SuperAdminRepository.GetAllAsync();

            return movies.Select(m => new MovieResponse
            {
                MovieId = m.MovieId,
                Title = m.Title,
                DurationMinutes = m.DurationMinutes,
                ReleaseDate = m.ReleaseDate,
                IsActive = m.IsActive,
                PosterUrl =m.PosterUrl
            }).ToList();
        }

        public async Task ToggleMovieAsync(Guid movieId)
        {
            var movie = await SuperAdminRepository.GetMovieByIdAsync(movieId);
            movie.IsActive = !movie.IsActive;
            await SuperAdminRepository.UpdateMovieAsync(movie);
        }
        public async Task AddTheatreAsync(CreateTheatreDto createTheatreDto, Guid superAdminId)
        {
            if (createTheatreDto.TimeSlots == null || !createTheatreDto.TimeSlots.Any())
                throw new InvalidOperationException(
                    "At least one show timing must be configured");

            // 1️⃣ Parse + normalize times
            var parsedSlots = createTheatreDto.TimeSlots.Select(ts =>
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
                Name = createTheatreDto.Name,
                Location = createTheatreDto.Location,
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
            await SuperAdminRepository.AddTheatreWithTimeSlotsAsync(theatre, timeSlots);
        }


        // This method is called by the Controller
        public async Task AddScreenAsync(CreateScreenRequest createScreenRequest)
        {
            if (createScreenRequest.SeatRows == null || !createScreenRequest.SeatRows.Any())
                throw new InvalidOperationException("Seat layout is required");

            // ✅ parse SeatLayoutType (string → enum)
            if (!Enum.TryParse<SeatLayoutType>(
                    createScreenRequest.SeatLayoutType, true, out var layoutType))
                throw new InvalidOperationException("Invalid seat layout type");

            var seatRows = new List<CreateSeatRowDto>();

            foreach (var row in createScreenRequest.SeatRows)
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
                TheatreId = createScreenRequest.TheatreId,
                ScreenName = createScreenRequest.ScreenName,
                IsActive =true,
                SeatLayoutType = layoutType,          // enum
                SeatRows = seatRows
            };

            await AddScreenInternalAsync(dto);
        }
        private async Task AddScreenInternalAsync(CreateScreenDto createScreenSto)
        {
            //  validations (enum-safe)
            if (createScreenSto.SeatRows.Select(r => r.SeatRow).Distinct().Count()
                != createScreenSto.SeatRows.Count)
                throw new InvalidOperationException("Duplicate seat rows are not allowed");

            var screen = new Screen
            {
                ScreenId = Guid.NewGuid(),
                TheatreId = createScreenSto.TheatreId,
                ScreenName = createScreenSto.ScreenName,
                SeatLayoutType = createScreenSto.SeatLayoutType, // enum 
                IsActive = true
            };

            await SuperAdminRepository.AddScreenAsync(screen);

            var seats = new List<Seat>();

            foreach (var row in createScreenSto.SeatRows)
            {
                for (int col = 1; col <= row.SeatCount; col++)
                {
                    seats.Add(new Seat
                    {
                        SeatId = Guid.NewGuid(),
                        ScreenId = screen.ScreenId,
                        SeatRow = row.SeatRow,
                        SeatColumn = col,
                        SeatType = row.SeatType,       // enum 
                        PriceMultiplier = row.PriceMultiplier,
                        IsActive = true
                    });
                }
            }

            await SuperAdminRepository.AddSeatsAsync(seats);
        }



        public async Task AddShowTimeAsync(CreateShowTimeDto createShowTimeDto)
        {
            // 1️⃣ Get theatre timings
            var slots = await SuperAdminRepository.GetTimeSlotsByTheatreAsync(createShowTimeDto.TheatreId);

            if (!slots.Any())
                throw new InvalidOperationException(
                    "Theatre has no configured time slots");

            var showTimes = new List<ShowTime>();

            foreach (var slot in slots)
            {
                var start = createShowTimeDto.ShowDate.ToDateTime(slot.StartTime);
                var end = createShowTimeDto.ShowDate.ToDateTime(slot.EndTime);

               // 2️ Business rule: no conflict per screen
                bool conflict = await SuperAdminRepository.ShowTimeConflictExistsAsync(
                    createShowTimeDto.ScreenId, start, end);

                if (conflict)
                    throw new InvalidOperationException(
                        "This screen is already scheduled for the selected date");

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

            // 3️⃣ Persist
            await SuperAdminRepository.AddShowTimesAsync(showTimes);
        }


        public async Task ApproveRequestAsync(Guid requestId)
        {
            var request = await SuperAdminRepository.GetRequestByIdAsync(requestId);
            request.Status = ApprovalStatus.APPROVED;
            request.ReviewedAt = DateTime.UtcNow;

            switch (request.RequestType)
            {
                case RequestType.THEATRE:
                    await SuperAdminRepository.ApproveTheatreAsync(request.ReferenceId);
                    break;
                case RequestType.SCREEN:
                    await SuperAdminRepository.ApproveScreenAsync(request.ReferenceId);
                    break;
                case RequestType.SHOWTIME:
                    await SuperAdminRepository.ApproveShowTimeAsync(request.ReferenceId);
                    break;
            }

            await SuperAdminRepository.UpdateRequestAsync(request);
        }

        public async Task RejectRequestAsync(Guid requestId)
        {
            var request = await SuperAdminRepository.GetRequestByIdAsync(requestId);
            request.Status = ApprovalStatus.REJECTED;
            request.ReviewedAt = DateTime.UtcNow;
            await SuperAdminRepository.UpdateRequestAsync(request);
        }

        public async Task AddLanguageAsync(CreateLanguageDto createLanguageDto)
        {
            var exists = await SuperAdminRepository.LanguageExistsAsync(createLanguageDto.Name);
            if (exists)
                throw new InvalidOperationException("Language already exists");

            var language = new Language
            {
                LanguageId = Guid.NewGuid(),
                Name = createLanguageDto.Name.Trim()
            };

            await SuperAdminRepository.AddLanguageAsync(language);
        }

        public async Task<List<LanguageDto>> GetLanguagesAsync()
        {
            var languages = await SuperAdminRepository.GetLanguagesAsync();
            return languages.Select(l => new LanguageDto
            {
                LanguageId = l.LanguageId,
                Name = l.Name
            }).ToList();
        }
        public async Task<List<TheatreResponseDto>> GetTheatresAsync()
        {
            var theatres = await SuperAdminRepository.GetTheatresAsync();

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
            var screens = await SuperAdminRepository.GetScreensAsync();

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
            var showTimes = await SuperAdminRepository.GetShowTimesAsync();

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

        public async Task<List<ScreenResponseDto>> GetScreensByTheatreAsync(Guid theatreId)
        {
            var screens = await SuperAdminRepository.GetByTheatreIdAsync(theatreId);

            return screens.Select(s => new ScreenResponseDto
            {
                ScreenId = s.ScreenId,
                TheatreId = s.TheatreId,
                ScreenName = s.ScreenName,
                SeatLayoutType = s.SeatLayoutType
            }).ToList();
        }
        // ========== UPDATE METHODS ==========

        public async Task UpdateMovieAsync(Guid movieId, UpdateMovieDto updateMovieDto)
        {
            var movie = await SuperAdminRepository.GetMovieByIdAsync(movieId);

            movie.Title = updateMovieDto.Title;
            movie.Description = updateMovieDto.Description;
            movie.DurationMinutes = updateMovieDto.DurationMinutes;
            movie.ReleaseDate = updateMovieDto.ReleaseDate;
            movie.PosterUrl = updateMovieDto.PosterUrl;

            await SuperAdminRepository.UpdateMovieAsync(movie);
        }

        public async Task UpdateTheatreAsync(Guid theatreId, UpdateTheatreDto updateTheatreDto)
        {
            if (updateTheatreDto.TimeSlots == null || !updateTheatreDto.TimeSlots.Any())
                throw new InvalidOperationException("At least one show timing must be configured");

            var theatre = await SuperAdminRepository.GetTheatreByIdAsync(theatreId);

            // Validate and parse time slots
            var parsedSlots = updateTheatreDto.TimeSlots.Select(ts =>
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
            theatre.Name = updateTheatreDto.Name;
            theatre.Location = updateTheatreDto.Location;

            // Delete existing time slots
            await SuperAdminRepository.DeleteTheatreTimeSlotsAsync(theatreId);

            // Create new time slots
            var newTimeSlots = parsedSlots.Select(p => new Domain.Entities.TheatreTimeSlot
            {
                TheatreTimeSlotId = Guid.NewGuid(),
                TheatreId = theatre.TheatreId,
                StartTime = p.Start,
                EndTime = p.End,
                IsActive = true
            }).ToList();

            await SuperAdminRepository.AddTheatreWithTimeSlotsAsync(theatre, newTimeSlots);
        }

        public async Task UpdateScreenAsync(Guid screenId, UpdateScreenDto updateScreenDto)
        {
            var screen = await SuperAdminRepository.GetScreenByIdAsync(screenId);

            // Check if screen has active showtimes
            var hasActiveShowTimes = await SuperAdminRepository.ScreenHasActiveShowTimesAsync(screenId);
            if (hasActiveShowTimes)
                throw new InvalidOperationException("Cannot update screen with active showtimes. Please deactivate or delete showtimes first.");

            if (updateScreenDto.SeatRows == null || !updateScreenDto.SeatRows.Any())
                throw new InvalidOperationException("Seat layout is required");

            // Parse SeatLayoutType
            if (!Enum.TryParse<SeatLayoutType>(updateScreenDto.SeatLayoutType, true, out var layoutType))
                throw new InvalidOperationException("Invalid seat layout type");

            var seatRows = new List<CreateSeatRowDto>();

            foreach (var row in updateScreenDto.SeatRows)
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
            screen.ScreenName = updateScreenDto.ScreenName;
            screen.SeatLayoutType = layoutType;

            await SuperAdminRepository.UpdateScreenAsync(screen);

            // Delete existing seats
            await SuperAdminRepository.DeleteScreenSeatsAsync(screenId);

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

            await SuperAdminRepository.AddSeatsAsync(seats);
        }

        public async Task UpdateShowTimeAsync(Guid showTimeId, UpdateShowTimeDto updateShowTimeDto)
        {
            var showTime = await SuperAdminRepository.GetShowTimeByIdAsync(showTimeId);

            // Get theatre time slots
            var slots = await SuperAdminRepository.GetTimeSlotsByTheatreAsync(showTime.TheatreId);

            if (!slots.Any())
                throw new InvalidOperationException("Theatre has no configured time slots");

            // Use the first slot (or you can modify logic to allow selecting a specific slot)
            var slot = slots.First();

            var start = updateShowTimeDto.ShowDate.ToDateTime(slot.StartTime);
            var end = updateShowTimeDto.ShowDate.ToDateTime(slot.EndTime);

            // Check for conflicts (excluding current showtime)
            var conflict = await SuperAdminRepository.ShowTimeConflictExistsAsync(showTime.ScreenId, start, end);

            if (conflict)
            {
                // Additional check: ensure it's not conflicting with itself
                var conflictingShowTime = await SuperAdminRepository.GetShowTimeByIdAsync(showTimeId);
                if (conflictingShowTime.ShowTimeId != showTimeId)
                    throw new InvalidOperationException("This screen is already scheduled for the selected date");
            }

            // Update showtime
            showTime.MovieId = updateShowTimeDto.MovieId;
            showTime.LanguageId = updateShowTimeDto.LanguageId;
            showTime.StartTime = start;
            showTime.EndTime = end;
            showTime.BasePrice = updateShowTimeDto.BasePrice;

            await SuperAdminRepository.UpdateShowTimeAsync(showTime);
        }

        public async Task UpdateLanguageAsync(Guid languageId, UpdateLanguageDto updatelanguageDto)
        {
            var language = await SuperAdminRepository.GetLanguageByIdAsync(languageId);

            // Check if name already exists (excluding current language)
            var exists = await SuperAdminRepository.LanguageExistsAsync(updatelanguageDto.Name);
            if (exists && !language.Name.Equals(updatelanguageDto.Name, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Language name already exists");

            language.Name = updatelanguageDto.Name.Trim();

            await SuperAdminRepository.UpdateLanguageAsync(language);
        }

        public async Task UpdateAdminAsync(Guid adminId, UpdateAdminDto UpdateAdminDto)
        {
            var admin = await SuperAdminRepository.GetUserByIdAsync(adminId);

            if (admin.Role != UserRole.Admin)
                throw new InvalidOperationException("User is not an admin");

            admin.Name = UpdateAdminDto.Name;
            admin.Email = UpdateAdminDto.Email;

            // Update password only if provided
            if (!string.IsNullOrWhiteSpace(UpdateAdminDto.Password))
            {
                admin.Password = BCrypt.Net.BCrypt.HashPassword(UpdateAdminDto.Password);
            }

            await SuperAdminRepository.UpdateUserAsync(admin);
        }

        // ========== DELETE METHODS ==========

        public async Task DeleteMovieAsync(Guid movieId)
        {
            var movie = await SuperAdminRepository.GetMovieByIdAsync(movieId);

            // Check if movie has active showtimes
            var hasActiveShowTimes = await SuperAdminRepository.MovieHasActiveShowTimesAsync(movieId);

            if (hasActiveShowTimes)
                throw new InvalidOperationException("Cannot delete movie with active showtimes. Please deactivate or delete showtimes first.");

            await SuperAdminRepository.DeleteMovieAsync(movie);
        }

        public async Task DeleteTheatreAsync(Guid theatreId)
        {
            var theatre = await SuperAdminRepository.GetTheatreByIdAsync(theatreId);

            // Check if theatre has active screens
            var hasActiveScreens = await SuperAdminRepository.TheatreHasActiveScreensAsync(theatreId);

            if (hasActiveScreens)
                throw new InvalidOperationException("Cannot delete theatre with active screens. Please deactivate or delete screens first.");

            // Delete time slots first (cascade)
            await SuperAdminRepository.DeleteTheatreTimeSlotsAsync(theatreId);

            await SuperAdminRepository.DeleteTheatreAsync(theatre);
        }

        public async Task DeleteScreenAsync(Guid screenId)
        {
            var screen = await SuperAdminRepository.GetScreenByIdAsync(screenId);

            // Check if screen has active showtimes
            var hasActiveShowTimes = await SuperAdminRepository.ScreenHasActiveShowTimesAsync(screenId);

            if (hasActiveShowTimes)
                throw new InvalidOperationException("Cannot delete screen with active showtimes. Please deactivate or delete showtimes first.");

            await SuperAdminRepository.DeleteScreenAsync(screen);
        }

        public async Task DeleteShowTimeAsync(Guid showTimeId)
        {
            var showTime = await SuperAdminRepository.GetShowTimeByIdAsync(showTimeId);

            // For now, simple delete
            await SuperAdminRepository.DeleteShowTimeAsync(showTime);
        }

        public async Task DeleteLanguageAsync(Guid languageId)
        {
            var language = await SuperAdminRepository.GetLanguageByIdAsync(languageId);

            // Check if language has active showtimes
            var hasActiveShowTimes = await SuperAdminRepository.LanguageHasActiveShowTimesAsync(languageId);

            if (hasActiveShowTimes)
                throw new InvalidOperationException("Cannot delete language with active showtimes. Please deactivate or delete showtimes first.");

            await SuperAdminRepository.DeleteLanguageAsync(language);
        }

        public async Task DeleteAdminAsync(Guid adminId)
        {
            var admin = await SuperAdminRepository.GetUserByIdAsync(adminId);

            if (admin.Role != UserRole.Admin)
                throw new InvalidOperationException("User is not an admin");

            // Check if admin has active theatres
            var hasActiveTheatres = await SuperAdminRepository.AdminHasActiveTheatresAsync(adminId);

            if (hasActiveTheatres)
                throw new InvalidOperationException("Cannot delete admin with active theatres. Please reassign or delete theatres first.");

            await SuperAdminRepository.DeleteAdminAsync(admin);
        }


        public async Task<MovieResponse> GetMovieByIdAsync(Guid movieId)
        {
            var movie = await SuperAdminRepository.GetMovieByIdAsync(movieId);

            return new MovieResponse
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                Description = movie.Description,
                DurationMinutes = movie.DurationMinutes,
                ReleaseDate = movie.ReleaseDate,
                PosterUrl=movie.PosterUrl,
                IsActive = movie.IsActive
            };
        }

        // ⭐ Theatre Details with TimeSlots
        public async Task<TheatreResponseDto> GetTheatreByIdAsync(Guid theatreId)
        {
            var theatre = await SuperAdminRepository.GetTheatreByIdAsync(theatreId);

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
            var screen = await SuperAdminRepository.GetScreenByIdAsync(screenId);
            var seats = await SuperAdminRepository.GetScreenSeatsAsync(screenId);

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

        public async Task<ShowTimeResponseDto> GetShowTimeByIdAsync(Guid showTimeId)
        {
            var showTime = await SuperAdminRepository.GetShowTimeByIdAsync(showTimeId);

            return new ShowTimeResponseDto
            {
                ShowTimeId = showTime.ShowTimeId,
                MovieTitle = showTime.Movie.Title,
                TheatreName = showTime.Theatre.Name,
                ScreenName = showTime.Screen.ScreenName,
                LanguageName = showTime.Language.Name,
                StartTime = showTime.StartTime,
                BasePrice = showTime.BasePrice,
            };
        }

        public async Task<LanguageDto> GetLanguageByIdAsync(Guid languageId)
        {
            var language = await SuperAdminRepository.GetLanguageByIdAsync(languageId);

            return new LanguageDto
            {
                LanguageId = language.LanguageId,
                Name = language.Name
            };
        }

        // ⭐ Admin Details
        public async Task<AdminDto> GetAdminByIdAsync(Guid adminId)
        {
            var admin = await SuperAdminRepository.GetUserByIdAsync(adminId);

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

        //
        public async Task<List<AdminRequestDto>> GetAllPendingRequestsAsync()
        {
            var requests = await SuperAdminRepository.GetAllPendingRequestsAsync();

            return requests.Select(r => new AdminRequestDto
            {
                AdminRequestId = r.AdminRequestId,
                RequestType = r.RequestType.ToString(),
                Status = r.Status.ToString(),
                RequestedAt = r.RequestedAt,
                ReviewedAt = r.ReviewedAt,
                RequestDetails = GetRequestDetails(r)
            }).ToList();
        }
        public async Task<List<AdminRequestDto>> GetAllRequestsAsync()
        {
            var requests = await SuperAdminRepository.GetAllRequestsAsync();

            return requests.Select(r => new AdminRequestDto
            {
                AdminRequestId = r.AdminRequestId,
                RequestType = r.RequestType.ToString(),
                Status = r.Status.ToString(),
                RequestedAt = r.RequestedAt,
                ReviewedAt = r.ReviewedAt,
                RequestDetails = GetRequestDetails(r)
            }).ToList();
        }

        public string GetRequestDetails(AdminRequest request)
        {
            // This would fetch the actual theatre/screen details based on ReferenceId
            return $"{request.RequestType} - Reference ID: {request.ReferenceId}";
        }

    }
}
