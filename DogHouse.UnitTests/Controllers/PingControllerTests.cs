using DogHouse.Controllers;
using DogHouse.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DogHouse.Tests.Controllers
{
    public class PingControllerTests
    {
        private readonly Mock<IDogService> _mockSvc;
        private readonly PingController _controller;

        public PingControllerTests()
        {
            _mockSvc = new Mock<IDogService>();
            _controller = new PingController(_mockSvc.Object);
        }

        [Fact]
        public async Task Ping_ShouldReturnOkWithVersion()
        {
            // Arrange
            var expected = "Dogshouseservice.Version1.0.1";
            _mockSvc.Setup(s => s.PingAsync()).ReturnsAsync(expected);

            // Act
            var result = await _controller.Ping();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().Be(expected);
            _mockSvc.Verify(s => s.PingAsync(), Times.Once);
        }
    }
}