using MovieBooking.Application.DTOs.Booking;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Interfaces.Services;
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
                        AvailableSeats = 0 // Will be calculated
                    }).ToList()
                }).ToList();

            // Calculate available seats for each show
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

        public async Task<LockSeatsResponseDto> LockSeatsAsync(Guid userId, LockSeatsRequestDto lockSeatsRequestDto)
        {
            // Validate seats are available
            // var available = await _bookingRepository.AreSeatsAvailableAsync(request.ShowTimeId, request.SeatIds);
            var activeLocks =
         await BookingRepository.GetActiveSeatLocksForShowAsync(
             lockSeatsRequestDto.ShowTimeId);
            if (activeLocks.Any(sl => lockSeatsRequestDto.SeatIds.Contains(sl.SeatId)))
            {
                return new LockSeatsResponseDto
                {
                    Success = false,
                    Message = "One or more seats are no longer available"
                };
            }

            // Lock the seats
            var locks = await BookingRepository.LockSeatsAsync(userId, lockSeatsRequestDto.ShowTimeId, lockSeatsRequestDto.SeatIds);

            // Get seat details for response
            var showTime = await BookingRepository.GetShowTimeByIdAsync(lockSeatsRequestDto.ShowTimeId);
            var seats = await BookingRepository.GetSeatsByScreenAsync(showTime.ScreenId);

            var lockedSeats = seats
                .Where(s => lockSeatsRequestDto.SeatIds.Contains(s.SeatId))
                .Select(s => new LockedSeatDto
                {
                    SeatId = s.SeatId,
                    SeatNumber = $"{s.SeatRow}{s.SeatColumn}",
                    Price = showTime.BasePrice * s.PriceMultiplier
                }).ToList();

            return new LockSeatsResponseDto
            {
                Success = true,
                Message = "Seats locked successfully",
                ExpiresAt = locks.First().ExpiresAt,
                LockedSeats = lockedSeats
            };
        }

        // ========== BOOKING & PAYMENT ==========

        public async Task<BookingConfirmationDto> CreateBookingAsync(Guid userId, CreateBookingRequestDto createBookingRequestDto)
        {

            // Verify seats are still available (locked by this user)
            //var available = await _bookingRepository.AreSeatsAvailableAsync(request.ShowTimeId, request.SeatIds);

            //if (!available)
            //    throw new InvalidOperationException("One or more seats are no longer available");

            var canBook = await BookingRepository
    .CanUserBookSeatsAsync(
        createBookingRequestDto.ShowTimeId,
        createBookingRequestDto.SeatIds,
        userId);

            if (!canBook)
            {
                throw new InvalidOperationException(
                    "You can only book seats locked by you");
            }

            var showTime = await BookingRepository.GetShowTimeByIdAsync(createBookingRequestDto.ShowTimeId);
            var seats = await BookingRepository.GetSeatsByScreenAsync(showTime.ScreenId);

            var selectedSeats = seats.Where(s => createBookingRequestDto.SeatIds.Contains(s.SeatId)).ToList();
            var totalAmount = selectedSeats.Sum(s => showTime.BasePrice * s.PriceMultiplier);

            // Create booking
            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                UserId = userId,
                ShowTimeId = createBookingRequestDto.ShowTimeId,
                TotalAmount = totalAmount,
                Status = BookingStatus.PENDING,
                CreatedAt = DateTime.UtcNow
            };

            await BookingRepository.CreateBookingAsync(booking);

            //Create booking seats
           var bookingSeats = selectedSeats.Select(s => new BookingSeat
           {
               BookingSeatId = Guid.NewGuid(),
               BookingId = booking.BookingId,
               SeatId = s.SeatId,
               ShowTimeId = createBookingRequestDto.ShowTimeId,
               SeatPrice = showTime.BasePrice * s.PriceMultiplier,
               Status = SeatLockStatus.LOCKED
           }).ToList();

            await BookingRepository.CreateBookingSeatsAsync(bookingSeats);

            // Return confirmation
            return new BookingConfirmationDto
            {
                BookingId = booking.BookingId,
                BookingReference = booking.BookingId.ToString().Substring(0, 8).ToUpper(),
                TotalAmount = totalAmount,
                Status = BookingStatus.PENDING.ToString(),//
                CreatedAt = booking.CreatedAt,
                Movie = new MovieDetailsDto
                {
                    Title = showTime.Movie.Title,
                    Language = showTime.Language.Name,
                    DurationMinutes = showTime.Movie.DurationMinutes
                },
                Theatre = new TheatreDetailsDto
                {
                    Name = showTime.Theatre.Name,
                    Location = showTime.Theatre.Location,
                    ScreenName = showTime.Screen.ScreenName,
                    ShowTime = showTime.StartTime
                },
                Seats = selectedSeats.Select(s => new BookedSeatDto
                {
                    SeatNumber = $"{s.SeatRow}{s.SeatColumn}",
                    SeatType = s.SeatType.ToString(),
                    Price = showTime.BasePrice * s.PriceMultiplier
                }).ToList()
            };
        }

        public async Task<PaymentResponseDto> ProcessPaymentAsync(Guid userId,ProcessPaymentRequestDto processPaymentRequestDto)
        {
            var booking = await BookingRepository
                .GetBookingByIdAsync(processPaymentRequestDto.BookingId);

            // 1️⃣ Verify booking belongs to user
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException(
                    "Unauthorized access to booking");

            // 2️⃣ Verify booking is pending
            if (booking.Status != BookingStatus.PENDING)
                throw new InvalidOperationException(
                    "Booking is not in pending status");

            // 3️⃣ Create payment record
            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                Amount = booking.TotalAmount,
                PaymentMethod = processPaymentRequestDto.PaymentMethod,
                PaymentStatus = PaymentStatus.PENDING,
                TransactionId = Guid.NewGuid().ToString(),
                PaymentGateway = processPaymentRequestDto.PaymentGateway,
                CreatedAt = DateTime.UtcNow
            };

            await BookingRepository.CreatePaymentAsync(payment);

            // 4️⃣ Simulate payment success
            payment.PaymentStatus = PaymentStatus.SUCCESS;
            payment.PaidAt = DateTime.UtcNow;
            await BookingRepository.UpdatePaymentAsync(payment);

            // 5️⃣ Confirm booking
            booking.Status = BookingStatus.CONFIRMED;
            booking.BookingTime = DateTime.UtcNow;
            booking.PaymentId = payment.PaymentId;
            await BookingRepository.UpdateBookingAsync(booking);

            // 6️⃣ Get locked seats for this booking
            var seatLocks = await BookingRepository
                .GetActiveSeatLocksForShowAsync(booking.ShowTimeId);

            var userSeatLocks = seatLocks
                .Where(sl =>
                    sl.UserId == userId &&
                    sl.Status == SeatLockStatus.LOCKED)
                .ToList();

            var seatIds = userSeatLocks.Select(sl => sl.SeatId).ToList();

           
            // 8️⃣ Convert seat locks (expire them)
            await BookingRepository.ConvertLocksToBookingAsync(
                userId,
                booking.ShowTimeId,
                seatIds);

            // 9️⃣ Send notification
            await BookingRepository.AddNotificationLogAsync(new NotificationLog
            {
                NotificationLogId = Guid.NewGuid(),
                UserId = userId,
                Type = NotificationType.EMAIL,
                Message = $"Booking confirmed for {booking.ShowTime.Movie.Title}",
                SentAt = DateTime.UtcNow,
                Status = NotificationStatus.SENT
            });

            // 🔟 Return response
            return new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                TransactionId = payment.TransactionId,
                Status = payment.PaymentStatus.ToString(),
                Amount = payment.Amount,
                PaidAt = payment.PaidAt,
                Message = "Payment successful"
            };
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
                SeatNumbers = string.Join(", ", b.BookingSeats.Select(bs => $"{bs.Seat.SeatRow}{bs.Seat.SeatColumn}")),
                TotalAmount = b.TotalAmount,
                Status = b.Status.ToString(),
                BookingDate = b.CreatedAt
            }).ToList();
        }

        public async Task<BookingConfirmationDto> GetBookingDetailsAsync(Guid userId, Guid bookingId)
        {
            var booking = await BookingRepository.GetBookingByIdAsync(bookingId);

            // Verify booking belongs to user
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("Unauthorized access to booking");

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

        public async Task CancelBookingAsync(Guid userId, CancelBookingRequestDto cancelBookingRequestDto)
        {
            var booking = await BookingRepository.GetBookingByIdAsync(cancelBookingRequestDto.BookingId);

            //// Verify booking belongs to user
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("Unauthorized access to booking");

            // Verify booking can be cancelled
            if (booking.Status != BookingStatus.CONFIRMED)
                throw new InvalidOperationException("Only confirmed bookings can be cancelled");

            // Check if show time has passed
            if (booking.ShowTime.StartTime <= DateTime.UtcNow)
                throw new InvalidOperationException("Cannot cancel booking for past shows");

            // Update booking status
            booking.Status = BookingStatus.CANCELLED;
            await BookingRepository.UpdateBookingAsync(booking);

            // If payment was made, initiate refund
            if (booking.PaymentId.HasValue)
            {
                var payment = await BookingRepository.GetPaymentByIdAsync(booking.PaymentId.Value);
                payment.PaymentStatus = PaymentStatus.REFUNDED;
                await BookingRepository.UpdatePaymentAsync(payment);
            }

            // Send notification
            await BookingRepository.AddNotificationLogAsync(new NotificationLog
            {
                NotificationLogId = Guid.NewGuid(),
                UserId = userId,
                Type = NotificationType.EMAIL,
                Message = $"Booking cancelled for {booking.ShowTime.Movie.Title}",
                SentAt = DateTime.UtcNow,
                Status = NotificationStatus.SENT
            });
        }

        // ========== BACKGROUND TASKS ==========

        public async Task ReleaseExpiredSeatLocksAsync()
        {
            await BookingRepository.ReleaseExpiredLocksAsync();
        }
    }
}
