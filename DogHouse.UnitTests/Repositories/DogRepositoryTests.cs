using DogHouse.Data;
using DogHouse.Models;
using DogHouse.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace DogHouse.UnitTests.Repositories
{
    public class DogRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly DogRepository _repo;

        public DogRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
            _repo = new DogRepository(_context);

            // Seed test data
            _context.Dogs.AddRange(
                new Dog { Name = "Neo", Color = "red&amber", TailLength = 22, Weight = 32 },
                new Dog { Name = "Jessy", Color = "black&white", TailLength = 7, Weight = 14 }
            );
            _context.SaveChanges();
        }

        public void Dispose() => _context.Dispose();

        [Fact]
        public async Task AddAsync_ShouldAddDogToDb()
        {
            // Arrange
            var newDog = new Dog { Name = "Buddy", Color = "brown", TailLength = 15, Weight = 25 };

            // Act
            await _repo.AddAsync(newDog);

            // Assert
            var saved = await _context.Dogs.FindAsync(newDog.Id);
            saved.Should().NotBeNull();
            saved.Name.Should().Be(newDog.Name);
            _context.Dogs.Count().Should().Be(3);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllDogs()
        {
            // Act
            var result = await _repo.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(d => d.Name == "Neo");
        }

        [Fact]
        public async Task GetByNameAsync_WithExistingName_ShouldReturnDog()
        {
            // Act
            var result = await _repo.GetByNameAsync("Neo");

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Neo");
        }

        [Fact]
        public async Task GetByNameAsync_WithNonExistingName_ShouldReturnNull()
        {
            // Act
            var result = await _repo.GetByNameAsync("Unknown");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task QueryAsync_WithoutAttribute_ShouldReturnPagedDogs()
        {
            // Act
            var (items, total) = await _repo.QueryAsync(null, null, 1, 1);

            // Assert
            total.Should().Be(2);
            items.Should().HaveCount(1); // pageSize=1
        }

        [Fact]
        public async Task QueryAsync_WithNameAsc_ShouldSortAscending()
        {
            // Act
            var (items, _) = await _repo.QueryAsync("name", "asc", 1, 2);

            // Assert
            items.Should().HaveCount(2);
            items.First().Name.Should().Be("Jessy"); // J < N
            items.Last().Name.Should().Be("Neo");
        }

        [Fact]
        public async Task QueryAsync_WithNameDesc_ShouldSortDescending()
        {
            // Act
            var (items, _) = await _repo.QueryAsync("name", "desc", 1, 2);

            // Assert
            items.Should().HaveCount(2);
            items.First().Name.Should().Be("Neo"); // N > J
            items.Last().Name.Should().Be("Jessy");
        }

        [Fact]
        public async Task QueryAsync_WithInvalidAttribute_ShouldIgnoreOrder()
        {
            // Act
            var (items, total) = await _repo.QueryAsync("invalid", "asc", 1, 2);

            // Assert
            total.Should().Be(2);
            items.Should().HaveCount(2);
        }

        [Fact]
        public async Task QueryAsync_WithPageNumberLessThan1_ShouldDefaultTo1()
        {
            // Act
            var (items, _) = await _repo.QueryAsync(null, null, 0, 2); // pageNumber=0 -> 1

            // Assert
            items.Should().HaveCount(2); // Full page
        }

        [Fact]
        public async Task QueryAsync_WithPageSizeLessThan1_ShouldDefaultTo10()
        {
            // Act
            var (items, _) = await _repo.QueryAsync(null, null, 1, 0); // pageSize=0 -> 10

            // Assert
            items.Should().HaveCount(2); // Less than 10
        }
    }
}