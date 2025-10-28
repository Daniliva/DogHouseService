using DogHouse.DTOs;
using DogHouse.Models;
using DogHouse.Repositories;
using DogHouse.Services;
using FluentAssertions;
using Moq;

namespace DogHouse.UnitTests.Services
{
    public class DogServiceTests
    {
        private readonly Mock<IDogRepository> _mockRepo;
        private readonly DogService _service;

        public DogServiceTests()
        {
            _mockRepo = new Mock<IDogRepository>();
            _service = new DogService(_mockRepo.Object);
        }

        [Fact]
        public async Task PingAsync_ShouldReturnVersionString()
        {
            // Act
            var result = await _service.PingAsync();

            // Assert
            result.Should().Be("Dogshouseservice.Version1.0.1");
        }

        [Fact]
        public async Task QueryDogsAsync_WithValidParams_ShouldMapToDtos()
        {
            // Arrange
            var dogs = new List<Dog>
            {
                new() { Id = 1, Name = "Neo", Color = "red&amber", TailLength = 22, Weight = 32 }
            };
            var expectedDtos = dogs.Select(d => new DogDto
            {
                Name = d.Name,
                Color = d.Color,
                TailLength = d.TailLength,
                Weight = d.Weight
            });
            _mockRepo.Setup(r => r.QueryAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((dogs, dogs.Count));

            // Act
            var (items, total) = await _service.QueryDogsAsync("name", "asc", 1, 10);

            // Assert
            items.Should().BeEquivalentTo(expectedDtos);
            total.Should().Be(dogs.Count);
            _mockRepo.Verify(r => r.QueryAsync("name", "asc", 1, 10), Times.Once);
        }

        [Fact]
        public async Task CreateDogAsync_WithValidDto_ShouldCreateAndReturnDto()
        {
            // Arrange
            var dto = new CreateDogDto { Name = "Buddy", Color = "brown", TailLength = 15, Weight = 25 };
            var newDog = new Dog { Id = 99, Name = dto.Name, Color = dto.Color, TailLength = dto.TailLength, Weight = dto.Weight };
            _mockRepo.Setup(r => r.GetByNameAsync(dto.Name)).ReturnsAsync((Dog?)null);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Dog>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateDogAsync(dto);

            // Assert
            result.Should().BeEquivalentTo(new DogDto
            {
                Name = dto.Name,
                Color = dto.Color,
                TailLength = dto.TailLength,
                Weight = dto.Weight
            });
            _mockRepo.Verify(r => r.GetByNameAsync(dto.Name), Times.Once);
            _mockRepo.Verify(r => r.AddAsync(It.Is<Dog>(d => d.Name == dto.Name)), Times.Once);
        }

        [Fact]
        public async Task CreateDogAsync_WithEmptyName_ShouldThrowArgumentException()
        {
            // Arrange
            var dto = new CreateDogDto { Name = "", Color = "brown", TailLength = 15, Weight = 25 };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateDogAsync(dto));
            ex.Message.Should().Be("Name is required");
        }

        [Fact]
        public async Task CreateDogAsync_WithNegativeTailLength_ShouldThrowArgumentException()
        {
            // Arrange
            var dto = new CreateDogDto { Name = "Buddy", Color = "brown", TailLength = -1, Weight = 25 };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateDogAsync(dto));
            ex.Message.Should().Be("TailLength must be non-negative");
        }

        [Fact]
        public async Task CreateDogAsync_WithExistingName_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var dto = new CreateDogDto { Name = "Neo", Color = "brown", TailLength = 15, Weight = 25 };
            _mockRepo.Setup(r => r.GetByNameAsync(dto.Name)).ReturnsAsync(new Dog { Name = dto.Name });

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateDogAsync(dto));
            ex.Message.Should().Be("Dog with the same name already exists");
        }
    }
}