using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieBooking.Api.Controllers;
using MovieBooking.Application.DTOs.Booking;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace MovieBooking.XunitTests.Controllers
{


    public class BookingControllerTests
    {
        private readonly Mock<IBookingService> ServiceMock;
        private readonly BookingController BookingController;

        public BookingControllerTests()
        {
            ServiceMock = new Mock<IBookingService>();
            BookingController = new BookingController(ServiceMock.Object);
        }

        private void SetUser(Guid userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }, "TestAuth"));

            BookingController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetShowTimes_InvalidDate_ReturnsBadRequest()
        {
            var result = await BookingController.GetShowTimes(Guid.NewGuid(), "invalid-date");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid date format. Use YYYY-MM-DD", badRequest.Value);
        }
        [Fact]
        public async Task GetShowTimes_ValidDate_CallsServiceOnce()
        {
            var movieId = Guid.NewGuid();
            var date = "2026-02-01";

            await BookingController.GetShowTimes(movieId, date);

            ServiceMock.Verify(
                s => s.GetShowTimesByMovieAsync(movieId, It.IsAny<DateOnly>()),
                Times.Once
            );
        }
        [Fact]
        public async Task LockSeats_Success_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            var request = new LockSeatsRequestDto();
            var response = new LockSeatsResponseDto { Success = true };

            ServiceMock
                .Setup(s => s.LockSeatsAsync(userId, request))
                .ReturnsAsync(response);

            var result = await BookingController.LockSeats(request);

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task LockSeats_Failure_ReturnsBadRequest()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            var request = new LockSeatsRequestDto();
            var response = new LockSeatsResponseDto { Success = false };

            ServiceMock
                .Setup(s => s.LockSeatsAsync(userId, request))
                .ReturnsAsync(response);

            var result = await BookingController.LockSeats(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task LockSeats_NoUserClaim_ThrowsException()
        {
            BookingController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            await Assert.ThrowsAsync<NullReferenceException>(
                async () => await BookingController.LockSeats(new LockSeatsRequestDto())
            );
        }
    }
}
