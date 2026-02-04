using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieBooking.Api.Controllers;
using MovieBooking.Application.DTOs.Admin;
using MovieBooking.Application.Interfaces.Services;
using System.Security.Claims;

namespace MovieBooking.XUnitTests.Integration
{
    public class AdminControllerTests
    {
        private readonly Mock<IAdminService> ServiceMock;
        private readonly AdminController AdminController;

        public AdminControllerTests()
        {
            ServiceMock = new Mock<IAdminService>();
            AdminController = new AdminController(ServiceMock.Object);

            SetAdminUser(Guid.NewGuid());
        }

        private void SetAdminUser(Guid adminId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, adminId.ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        }, "TestAuth"));

            AdminController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
        [Fact]
        public async Task RequestTheatre_ValidRequest_ReturnsOk()
        {
            var dto = new CreateTheatreRequestDto();
            var theatreId = Guid.NewGuid();

            ServiceMock
                .Setup(s => s.RequestTheatreAsync(dto, It.IsAny<Guid>()))
                .ReturnsAsync(theatreId);

            var result = await AdminController.RequestTheatre(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Theatre request submitted successfully",
                ok.Value!.ToString());
        }
        

    }

}
