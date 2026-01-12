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

        public async Task ToggleMovieAsync(Guid movieId)
        {
            var movie = await _repo.GetMovieByIdAsync(movieId);
            movie.IsActive = !movie.IsActive;
            await _repo.UpdateMovieAsync(movie);
        }

        public async Task AddTheatreAsync(CreateTheatreDto dto, Guid superAdminId)
        {
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

            await _repo.AddTheatreAsync(theatre);
        }

        public async Task AddScreenAsync(CreateScreenDto dto)
        {
            var screen = new Screen
            {
                ScreenId = Guid.NewGuid(),
                TheatreId = dto.TheatreId,
                ScreenName = dto.ScreenName,
                SeatLayoutType = dto.SeatLayoutType,
                IsActive = true
            };

            await _repo.AddScreenAsync(screen);
        }

        public async Task AddShowTimeAsync(CreateShowTimeDto dto)
        {
            //  Validate Language
            var language = await _repo.GetLanguageByIdAsync(dto.LanguageId);
            if (language == null)
                throw new InvalidOperationException("Invalid language");

            var showTime = new ShowTime
            {
                ShowTimeId = Guid.NewGuid(),
                MovieId = dto.MovieId,
                TheatreId = dto.TheatreId,
                ScreenId = dto.ScreenId,
                LanguageId = dto.LanguageId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                BasePrice = dto.BasePrice,
                ApprovalStatus = ApprovalStatus.APPROVED,
                IsActive = true
            };

            await _repo.AddShowTimeAsync(showTime);
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

    }
}
