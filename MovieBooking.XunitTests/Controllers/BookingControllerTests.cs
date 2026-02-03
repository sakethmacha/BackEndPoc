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
        private readonly Mock<IBookingService> _serviceMock;
        private readonly BookingController _controller;

        public BookingControllerTests()
        {
            _serviceMock = new Mock<IBookingService>();
            _controller = new BookingController(_serviceMock.Object);
        }

        private void SetUser(Guid userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }, "TestAuth"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    //    [Fact]
    //    public async Task GetShowTimes_ValidDate_ReturnsOk()
    //    {
    //        var movieId = Guid.NewGuid();
    //        var date = "2026-02-01";

    //        _serviceMock
    //.Setup(s => s.GetShowTimesByMovieAsync(movieId, It.IsAny<DateOnly>()))
    //.ReturnsAsync(new List<TheatreShowDto>
    //{
    //    new TheatreShowDto
    //    {
    //        TheatreId = Guid.NewGuid(),
    //        TheatreName = "PVR Cinemas",
    //        Location = "Hyderabad",
    //        Shows = new List<ShowDto>
    //        {
    //            new ShowDto
    //            {
    //                ShowDate = "10:00 AM"
    //            },
    //            new ShowDto
    //            {
    //                ShowTime = "01:00 PM"
    //            }
    //        }
    //    }
    //});



        //    var result = await _controller.GetShowTimes(movieId, date);

        //    var ok = Assert.IsType<OkObjectResult>(result);
        //    Assert.NotNull(ok.Value);
        //}
        [Fact]
        public async Task GetShowTimes_InvalidDate_ReturnsBadRequest()
        {
            var result = await _controller.GetShowTimes(Guid.NewGuid(), "invalid-date");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid date format. Use YYYY-MM-DD", badRequest.Value);
        }
        [Fact]
        public async Task GetShowTimes_ValidDate_CallsServiceOnce()
        {
            var movieId = Guid.NewGuid();
            var date = "2026-02-01";

            await _controller.GetShowTimes(movieId, date);

            _serviceMock.Verify(
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

            _serviceMock
                .Setup(s => s.LockSeatsAsync(userId, request))
                .ReturnsAsync(response);

            var result = await _controller.LockSeats(request);

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task LockSeats_Failure_ReturnsBadRequest()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            var request = new LockSeatsRequestDto();
            var response = new LockSeatsResponseDto { Success = false };

            _serviceMock
                .Setup(s => s.LockSeatsAsync(userId, request))
                .ReturnsAsync(response);

            var result = await _controller.LockSeats(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task LockSeats_NoUserClaim_ThrowsException()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            await Assert.ThrowsAsync<NullReferenceException>(
                async () => await _controller.LockSeats(new LockSeatsRequestDto())
            );
        }

    }
}
