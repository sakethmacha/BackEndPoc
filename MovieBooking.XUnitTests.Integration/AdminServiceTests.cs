using Moq;
using MovieBooking.Application.DTOs.Admin;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Application.Services;
using MovieBooking.Domain.Entities;
namespace MovieBooking.XUnitTests.Integration
{
    public class AdminServiceTests
    {
        private readonly Mock<IAdminRepository> _repoMock;
        private readonly AdminService _service;

        public AdminServiceTests()
        {
            _repoMock = new Mock<IAdminRepository>();
            _service = new AdminService(_repoMock.Object);
        }
        [Fact]
        public async Task RequestTheatreAsync_NoTimeSlots_ThrowsException()
        {
            var dto = new CreateTheatreRequestDto
            {
                Name = "Test Theatre",
                Location = "City",
                TimeSlots = new List<TimeSlotResponseDto>()
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.RequestTheatreAsync(dto, Guid.NewGuid()));
        }
        [Fact]
        public async Task RequestTheatreAsync_InvalidTimeFormat_ThrowsException()
        {
            var dto = new CreateTheatreRequestDto
            {
                Name = "Test",
                Location = "City",
                TimeSlots = new()
        {
            new TimeSlotResponseDto { StartTime = "25:00", EndTime = "26:00" }
        }
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.RequestTheatreAsync(dto, Guid.NewGuid()));
        }
        [Fact]
        public async Task RequestTheatreAsync_OverlappingSlots_ThrowsException()
        {
            var dto = new CreateTheatreRequestDto
            {
                Name = "Test",
                Location = "City",
                TimeSlots = new()
        {
            new TimeSlotResponseDto { StartTime = "10:00", EndTime = "12:00" },
            new TimeSlotResponseDto { StartTime = "11:00", EndTime = "13:00" }
        }
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.RequestTheatreAsync(dto, Guid.NewGuid()));
        }
        [Fact]
        public async Task RequestTheatreAsync_ValidRequest_ReturnsTheatreId()
        {
            var adminId = Guid.NewGuid();
            var theatreId = Guid.NewGuid();

            _repoMock
                .Setup(r => r.CreateTheatreRequestAsync(
                    It.IsAny<Theatre>(),
                    It.IsAny<List<TheatreTimeSlot>>(),
                    It.IsAny<AdminRequest>()))
                .ReturnsAsync(theatreId);

            var dto = new CreateTheatreRequestDto
            {
                Name = "Test Theatre",
                Location = "City",
                TimeSlots = new()
        {
            new TimeSlotResponseDto { StartTime = "10:00", EndTime = "12:00" }
        }
            };

            var result = await _service.RequestTheatreAsync(dto, adminId);

            Assert.Equal(theatreId, result);
        }

    }

}
