using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieBooking.Api.Controllers;
using MovieBooking.Application.DTOs.SuperAdmin;
using MovieBooking.Application.Interfaces.Services;

namespace MovieBooking.Tests.Controllers
{
    [TestFixture]
    public class SuperAdminControllerTests
    {
        private Mock<ISuperAdminService> _serviceMock;
        private SuperAdminController _controller;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<ISuperAdminService>();
            _controller = new SuperAdminController(_serviceMock.Object);
        }

        //  Test 1: Returns OkResult
        [Test]
        public async Task AddMovie_ValidDto_ReturnsOk()
        {
            var dto = new AddMovieDto { Title = "Inception" };

            var result = await _controller.AddMovie(dto);

            Assert.IsInstanceOf<OkResult>(result);
        }

        //  Test 2: Calls service once
        [Test]
        public async Task AddMovie_ValidDto_CallsServiceOnce()
        {
            var dto = new AddMovieDto { Title = "Inception" };

            await _controller.AddMovie(dto);

            _serviceMock.Verify(
                s => s.AddMovieAsync(dto),
                Times.Once
            );
        }

        //  Test 3: Passes correct DTO
        [Test]
        public async Task AddMovie_PassesCorrectDtoToService()
        {
            var dto = new AddMovieDto { Title = "Interstellar" };

            await _controller.AddMovie(dto);

            _serviceMock.Verify(
                s => s.AddMovieAsync(It.Is<AddMovieDto>(
                    d => d.Title == "Interstellar"
                )),
                Times.Once
            );
        }

        //  Test 4: Works with null DTO (current behavior)
        [Test]
        public async Task AddMovie_NullDto_ReturnsOk()
        {
            var result = await _controller.AddMovie(null);

            Assert.IsInstanceOf<OkResult>(result);
        }

        //  Test 5: Exception bubbles up
        [Test]
        public void AddMovie_ServiceThrowsException_ThrowsException()
        {
            var dto = new AddMovieDto();

            _serviceMock
                .Setup(s => s.AddMovieAsync(dto))
                .ThrowsAsync(new Exception("DB error"));

            Assert.ThrowsAsync<Exception>(
                async () => await _controller.AddMovie(dto)
            );
        }
    }

}
