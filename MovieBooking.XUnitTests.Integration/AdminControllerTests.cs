using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MovieBooking.Api.Controllers;
using MovieBooking.Application.DTOs.Admin;
using MovieBooking.Application.Interfaces.Services;
using MovieBooking.Infrastructure.Repositories;
using System.Security.Claims;

namespace MovieBooking.XUnitTests.Integration
{
    public class AdminControllerTests
    {
        private readonly Mock<IAdminService> _serviceMock;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _serviceMock = new Mock<IAdminService>();
            _controller = new AdminController(_serviceMock.Object);

            SetAdminUser(Guid.NewGuid());
        }

        private void SetAdminUser(Guid adminId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, adminId.ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        }, "TestAuth"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
        [Fact]
        public async Task RequestTheatre_ValidRequest_ReturnsOk()
        {
            var dto = new CreateTheatreRequestDto();
            var theatreId = Guid.NewGuid();

            _serviceMock
                .Setup(s => s.RequestTheatreAsync(dto, It.IsAny<Guid>()))
                .ReturnsAsync(theatreId);

            var result = await _controller.RequestTheatre(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Theatre request submitted successfully",
                ok.Value!.ToString());
        }
        

    }

}
