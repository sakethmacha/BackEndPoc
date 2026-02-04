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
        private Mock<ISuperAdminService> ServiceMock;
        private SuperAdminController SuperAdminController;

        [SetUp]
        public void Setup()
        {
            ServiceMock = new Mock<ISuperAdminService>();
            SuperAdminController = new SuperAdminController(ServiceMock.Object);
        }

        //  Returns OkResult
        [Test]
        public async Task AddMovie_ValidDto_ReturnsOk()
        {
            var dto = new AddMovieDto { Title = "Inception" };

            var result = await SuperAdminController.AddMovie(dto);

            Assert.IsInstanceOf<OkResult>(result);
        }

        //  Calls service once
        [Test]
        public async Task AddMovie_ValidDto_CallsServiceOnce()
        {
            var dto = new AddMovieDto { Title = "Inception" };

            await SuperAdminController.AddMovie(dto);

            ServiceMock.Verify(
                s => s.AddMovieAsync(dto),
                Times.Once
            );
        }

        //  Passes correct DTO
        [Test]
        public async Task AddMovie_PassesCorrectDtoToService()
        {
            var dto = new AddMovieDto { Title = "Interstellar" };

            await SuperAdminController.AddMovie(dto);

            ServiceMock.Verify(
                s => s.AddMovieAsync(It.Is<AddMovieDto>(
                    d => d.Title == "Interstellar"
                )),
                Times.Once
            );
        }

        //  Works with null DTO (current behavior)
        [Test]
        public async Task AddMovie_NullDto_ReturnsOk()
        {
            var result = await SuperAdminController.AddMovie(null);

            Assert.IsInstanceOf<OkResult>(result);
        }

        //  Exception bubbles up
        [Test]
        public void AddMovie_ServiceThrowsException_ThrowsException()
        {
            var dto = new AddMovieDto();

            ServiceMock
                .Setup(s => s.AddMovieAsync(dto))
                .ThrowsAsync(new Exception("DB error"));

            Assert.ThrowsAsync<Exception>(
                async () => await SuperAdminController.AddMovie(dto)
            );
        }
    }

}
