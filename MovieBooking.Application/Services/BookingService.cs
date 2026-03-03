using MovieBooking.Application.DTOs.Booking;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Domain.Constants;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;

namespace MovieBooking.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository BookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            BookingRepository = bookingRepository;
        }

        // ========== BROWSE MOVIES & SHOWS ==========

        public async Task<List<MovieListDto>> GetActiveMoviesAsync()
        {
            var movies = await BookingRepository.GetActiveMoviesAsync();

            return movies.Select(m => new MovieListDto
            {
                MovieId = m.MovieId,
                Title = m.Title,
                Description = m.Description,
                DurationMinutes = m.DurationMinutes,
                ReleaseDate = m.ReleaseDate,
                PosterUrl = m.PosterUrl
            }).ToList();
        }

        public async Task<List<TheatreShowDto>> GetShowTimesByMovieAsync(Guid movieId, DateOnly date)
        {
            var showTimes = await BookingRepository.GetShowTimesByMovieAsync(movieId, date);

            var grouped = showTimes
                .GroupBy(st => new { st.TheatreId, st.Theatre.Name, st.Theatre.Location })
                .Select(g => new TheatreShowDto
                {
                    TheatreId = g.Key.TheatreId,
                    TheatreName = g.Key.Name,
                    Location = g.Key.Location,
                    Shows = g.Select(st => new ShowDto
                    {
                        ShowTimeId = st.ShowTimeId,
                        ScreenId = st.ScreenId,
                        ScreenName = st.Screen.ScreenName,
                        LanguageName = st.Language.Name,
                        StartTime = st.StartTime,
                        EndTime = st.EndTime,
                        BasePrice = st.BasePrice,
                        AvailableSeats = 0
                    }).ToList()
                }).ToList();

            foreach (var theatre in grouped)
            {
                foreach (var show in theatre.Shows)
                {
                    var totalSeats = (await BookingRepository.GetSeatsByScreenAsync(show.ScreenId)).Count;
                    var bookedSeats = (await BookingRepository.GetBookedSeatsForShowAsync(show.ShowTimeId)).Count;
                    var lockedSeats = (await BookingRepository.GetActiveSeatLocksForShowAsync(show.ShowTimeId)).Count;
                    show.AvailableSeats = totalSeats - bookedSeats - lockedSeats;
                }
            }

            return grouped;
        }

        // ========== SEAT SELECTION ==========

        public async Task<SeatLayoutDto> GetSeatLayoutAsync(Guid showTimeId)
        {
            var showTime = await BookingRepository.GetShowTimeByIdAsync(showTimeId);
            var seats = await BookingRepository.GetSeatsByScreenAsync(showTime.ScreenId);
            var bookedSeats = await BookingRepository.GetBookedSeatsForShowAsync(showTimeId);
            var lockedSeats = await BookingRepository.GetActiveSeatLocksForShowAsync(showTimeId);

            var bookedSeatIds = bookedSeats.Select(bs => bs.SeatId).ToHashSet();
            var lockedSeatIds = lockedSeats.Select(ls => ls.SeatId).ToHashSet();

            var seatRows = seats
                .GroupBy(s => s.SeatRow)
                .OrderBy(g => g.Key)
                .Select(g => new SeatRowDto
                {
                    RowName = g.Key,
                    Seats = g.OrderBy(s => s.SeatColumn).Select(s => new SeatDto
                    {
                        SeatId = s.SeatId,
                        SeatRow = s.SeatRow,
                        SeatColumn = s.SeatColumn,
                        SeatNumber = $"{s.SeatRow}{s.SeatColumn}",
                        SeatType = s.SeatType.ToString(),
                        Price = showTime.BasePrice * s.PriceMultiplier,
                        IsAvailable = !bookedSeatIds.Contains(s.SeatId) && !lockedSeatIds.Contains(s.SeatId),
                        IsLocked = lockedSeatIds.Contains(s.SeatId)
                    }).ToList()
                }).ToList();

            return new SeatLayoutDto
            {
                ShowTimeId = showTimeId,
                MovieTitle = showTime.Movie.Title,
                TheatreName = showTime.Theatre.Name,
                ScreenName = showTime.Screen.ScreenName,
                StartTime = showTime.StartTime,
                BasePrice = showTime.BasePrice,
                SeatRows = seatRows
            };
        }

        public async Task<LockSeatsResponseDto> LockSeatsAsync(Guid userId, LockSeatsRequestDto request)
        {
            var activeLocks =
                await BookingRepository.GetActiveSeatLocksForShowAsync(request.ShowTimeId);

            if (activeLocks.Any(sl => request.SeatIds.Contains(sl.SeatId)))
            {
                return new LockSeatsResponseDto
                {
                    Success = false,
                    Message = MessageStrings.SeatsNoLongerAvailable
                };
            }

            var locks = await BookingRepository.LockSeatsAsync(userId, request.ShowTimeId, request.SeatIds);

            var showTime = await BookingRepository.GetShowTimeByIdAsync(request.ShowTimeId);
            var seats = await BookingRepository.GetSeatsByScreenAsync(showTime.ScreenId);

            var lockedSeats = seats
                .Where(s => request.SeatIds.Contains(s.SeatId))
                .Select(s => new LockedSeatDto
                {
                    SeatId = s.SeatId,
                    SeatNumber = $"{s.SeatRow}{s.SeatColumn}",
                    Price = showTime.BasePrice * s.PriceMultiplier
                }).ToList();

            return new LockSeatsResponseDto
            {
                Success = true,
                Message = MessageStrings.SeatsLockedSuccessfully,
                ExpiresAt = locks.First().ExpiresAt,
                LockedSeats = lockedSeats
            };
        }

        // ========== BOOKING & PAYMENT ==========

        public async Task<BookingConfirmationDto> CreateBookingAsync(Guid userId, CreateBookingRequestDto request)
        {
            var canBook = await BookingRepository
                .CanUserBookSeatsAsync(request.ShowTimeId, request.SeatIds, userId);

            if (!canBook)
                throw new InvalidOperationException(
                    MessageStrings.CanOnlyBookLockedSeats);

            var showTime = await BookingRepository.GetShowTimeByIdAsync(request.ShowTimeId);
            var seats = await BookingRepository.GetSeatsByScreenAsync(showTime.ScreenId);

            var selectedSeats = seats.Where(s => request.SeatIds.Contains(s.SeatId)).ToList();
            var totalAmount = selectedSeats.Sum(s => showTime.BasePrice * s.PriceMultiplier);

            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                UserId = userId,
                ShowTimeId = request.ShowTimeId,
                TotalAmount = totalAmount,
                Status = BookingStatus.PENDING,
                CreatedAt = DateTime.UtcNow
            };

            await BookingRepository.CreateBookingAsync(booking);

            var bookingSeats = selectedSeats.Select(s => new BookingSeat
            {
                BookingSeatId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                SeatId = s.SeatId,
                ShowTimeId = request.ShowTimeId,
                SeatPrice = showTime.BasePrice * s.PriceMultiplier,
                Status = SeatLockStatus.LOCKED
            }).ToList();

            await BookingRepository.CreateBookingSeatsAsync(bookingSeats);

            return new BookingConfirmationDto
            {
                BookingId = booking.BookingId,
                BookingReference = booking.BookingId.ToString().Substring(0, 8).ToUpper(),
                TotalAmount = totalAmount,
                Status = BookingStatus.PENDING.ToString(),
                CreatedAt = booking.CreatedAt
            };
        }

        public async Task<PaymentResponseDto> ProcessPaymentAsync(Guid userId, ProcessPaymentRequestDto request)
        {
            var booking = await BookingRepository.GetBookingByIdAsync(request.BookingId);

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException(
                    MessageStrings.UnauthorizedBookingAccess);

            if (booking.Status != BookingStatus.PENDING)
                throw new InvalidOperationException(
                    MessageStrings.BookingNotPending);

            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                Amount = booking.TotalAmount,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = PaymentStatus.SUCCESS,
                TransactionId = Guid.NewGuid().ToString(),
                PaymentGateway = request.PaymentGateway,
                CreatedAt = DateTime.UtcNow,
                PaidAt = DateTime.UtcNow
            };

            await BookingRepository.CreatePaymentAsync(payment);

            booking.Status = BookingStatus.CONFIRMED;
            booking.BookingTime = DateTime.UtcNow;
            booking.PaymentId = payment.PaymentId;
            await BookingRepository.UpdateBookingAsync(booking);

            await BookingRepository.AddNotificationLogAsync(new NotificationLog
            {
                NotificationLogId = Guid.NewGuid(),
                UserId = userId,
                Type = NotificationType.EMAIL,
                Message = $"{MessageStrings.BookingConfirmedFor} {booking.ShowTime.Movie.Title}",
                SentAt = DateTime.UtcNow,
                Status = NotificationStatus.SENT
            });

            return new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                TransactionId = payment.TransactionId,
                Status = payment.PaymentStatus.ToString(),
                Amount = payment.Amount,
                PaidAt = payment.PaidAt,
                Message = MessageStrings.PaymentSuccessful
            };
        }

        public async Task CancelBookingAsync(Guid userId, CancelBookingRequestDto request)
        {
            var booking = await BookingRepository.GetBookingByIdAsync(request.BookingId);

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException(
                    MessageStrings.UnauthorizedBookingAccess);

            if (booking.Status != BookingStatus.CONFIRMED)
                throw new InvalidOperationException(
                    MessageStrings.OnlyConfirmedBookingsCanBeCancelled);

            if (booking.ShowTime.StartTime <= DateTime.UtcNow)
                throw new InvalidOperationException(
                    MessageStrings.CannotCancelPastShow);

            booking.Status = BookingStatus.CANCELLED;
            await BookingRepository.UpdateBookingAsync(booking);

            if (booking.PaymentId.HasValue)
            {
                var payment = await BookingRepository.GetPaymentByIdAsync(booking.PaymentId.Value);
                payment.PaymentStatus = PaymentStatus.REFUNDED;
                await BookingRepository.UpdatePaymentAsync(payment);
            }

            await BookingRepository.AddNotificationLogAsync(new NotificationLog
            {
                NotificationLogId = Guid.NewGuid(),
                UserId = userId,
                Type = NotificationType.EMAIL,
                Message = $"{MessageStrings.BookingCancelledFor} {booking.ShowTime.Movie.Title}",
                SentAt = DateTime.UtcNow,
                Status = NotificationStatus.SENT
            });
        }

        public async Task ReleaseExpiredSeatLocksAsync()
        {
            await BookingRepository.ReleaseExpiredLocksAsync();
        }

        // ========== USER BOOKINGS ==========

        public async Task<List<UserBookingDto>> GetUserBookingsAsync(Guid userId)
        {
            var bookings = await BookingRepository.GetUserBookingsAsync(userId);

            return bookings.Select(b => new UserBookingDto
            {
                BookingId = b.BookingId,
                BookingReference = b.BookingId.ToString().Substring(0, 8).ToUpper(),
                MovieTitle = b.ShowTime.Movie.Title,
                TheatreName = b.ShowTime.Theatre.Name,
                ScreenName = b.ShowTime.Screen.ScreenName,
                ShowTime = b.ShowTime.StartTime,
                SeatCount = b.BookingSeats.Count,
                SeatNumbers = string.Join(", ",
                    b.BookingSeats.Select(bs =>
                        $"{bs.Seat.SeatRow}{bs.Seat.SeatColumn}")),
                TotalAmount = b.TotalAmount,
                Status = b.Status.ToString(),
                BookingDate = b.CreatedAt
            }).ToList();
        }


        public async Task<BookingConfirmationDto> GetBookingDetailsAsync(Guid userId, Guid bookingId)
        {
            var booking = await BookingRepository.GetBookingByIdAsync(bookingId);

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException(
                    MessageStrings.UnauthorizedBookingAccess);

            return new BookingConfirmationDto
            {
                BookingId = booking.BookingId,
                BookingReference = booking.BookingId.ToString().Substring(0, 8).ToUpper(),
                TotalAmount = booking.TotalAmount,
                Status = booking.Status.ToString(),
                CreatedAt = booking.CreatedAt,
                Movie = new MovieDetailsDto
                {
                    Title = booking.ShowTime.Movie.Title,
                    Language = booking.ShowTime.Language.Name,
                    DurationMinutes = booking.ShowTime.Movie.DurationMinutes
                },
                Theatre = new TheatreDetailsDto
                {
                    Name = booking.ShowTime.Theatre.Name,
                    Location = booking.ShowTime.Theatre.Location,
                    ScreenName = booking.ShowTime.Screen.ScreenName,
                    ShowTime = booking.ShowTime.StartTime
                },
                Seats = booking.BookingSeats.Select(bs => new BookedSeatDto
                {
                    SeatNumber = $"{bs.Seat.SeatRow}{bs.Seat.SeatColumn}",
                    SeatType = bs.Seat.SeatType.ToString(),
                    Price = bs.SeatPrice
                }).ToList()
            };
        }
    }
}