using DogHouse.Controllers;
using DogHouse.DTOs;
using DogHouse.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DogHouse.Tests.Controllers
{
    public class DogsControllerTests
    {
        private readonly Mock<IDogService> _mockSvc;
        private readonly DogsController _controller;

        public DogsControllerTests()
        {
            _mockSvc = new Mock<IDogService>();
            _controller = new DogsController(_mockSvc.Object);
        }

        [Fact]
        public async Task GetDogs_WithValidParams_ShouldReturnOkWithItems()
        {
            // Arrange
            var dtos = new List<DogDto> { new() { Name = "Neo", Color = "red&amber", TailLength = 22, Weight = 32 } };
            var total = 1;
            _mockSvc.Setup(s => s.QueryDogsAsync("name", "asc", 1, 10))
                .ReturnsAsync((dtos, total));

            // Act
            var result = await _controller.GetDogs("name", "asc", 1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(dtos);
            _mockSvc.Verify(s => s.QueryDogsAsync("name", "asc", 1, 10), Times.Once);
        }

        [Fact]
        public async Task GetDogs_WithDefaultParams_ShouldReturnOkWithItems()
        {
            // Arrange
            var dtos = new List<DogDto> { new() { Name = "Jessy", Color = "black&white", TailLength = 7, Weight = 14 } };
            _mockSvc.Setup(s => s.QueryDogsAsync(null, null, 1, 100))
                .ReturnsAsync((dtos, dtos.Count));

            // Act
            var result = await _controller.GetDogs(null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(dtos);
        }

        [Fact]
        public async Task CreateDog_WithValidDto_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var dto = new CreateDogDto { Name = "Buddy", Color = "brown", TailLength = 15, Weight = 25 };
            var createdDto = new DogDto { Name = dto.Name, Color = dto.Color, TailLength = dto.TailLength, Weight = dto.Weight };
            _mockSvc.Setup(s => s.CreateDogAsync(dto)).ReturnsAsync(createdDto);

            // Act
            var result = await _controller.CreateDog(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            createdResult.Value.Should().Be(createdDto);
            createdResult.ActionName.Should().Be(nameof(_controller.GetDogs));
            _mockSvc.Verify(s => s.CreateDogAsync(dto), Times.Once);
        }

        [Fact]
        public async Task CreateDog_WithNullDto_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.CreateDog(null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            badRequest.Value.Should().Be("Invalid JSON or empty body");
        }

        [Fact]
        public async Task CreateDog_WithArgumentException_ShouldReturnBadRequest()
        {
            // Arrange
            var dto = new CreateDogDto { Name = "", Color = "brown", TailLength = 15, Weight = 25 };
            _mockSvc.Setup(s => s.CreateDogAsync(dto)).ThrowsAsync(new ArgumentException("Name is required"));

            // Act
            var result = await _controller.CreateDog(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            badRequest.Value.Should().Be("Name is required");
        }

        [Fact]
        public async Task CreateDog_WithInvalidOperationException_ShouldReturnConflict()
        {
            // Arrange
            var dto = new CreateDogDto { Name = "Neo", Color = "brown", TailLength = 15, Weight = 25 };
            _mockSvc.Setup(s => s.CreateDogAsync(dto)).ThrowsAsync(new InvalidOperationException("Dog exists"));

            // Act
            var result = await _controller.CreateDog(dto);

            // Assert
            var conflict = Assert.IsType<ConflictObjectResult>(result);
            conflict.Value.Should().Be("Dog exists");
        }
    }
}