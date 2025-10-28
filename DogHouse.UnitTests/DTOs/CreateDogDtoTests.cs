using DogHouse.DTOs;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace DogHouse.Tests.DTOs
{
    public class CreateDogDtoTests
    {
        [Fact]
        public void Validate_WithValidDto_ShouldBeValid()
        {
            // Arrange
            var dto = new CreateDogDto
            {
                Name = "Buddy",
                Color = "brown",
                TailLength = 15,
                Weight = 25
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void Validate_WithEmptyName_ShouldBeInvalid()
        {
            // Arrange
            var dto = new CreateDogDto
            {
                Name = "",
                Color = "brown",
                TailLength = 15,
                Weight = 25
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(dto, context, results, true);

            // Assert
            results.Should().HaveCount(1);
            results[0].ErrorMessage.Should().Contain("The Name field is required.");
        }

        [Fact]
        public void Validate_WithLongName_ShouldBeInvalid()
        {
            // Arrange
            var dto = new CreateDogDto
            {
                Name = new string('A', 101), // > 100
                Color = "brown",
                TailLength = 15,
                Weight = 25
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(dto, context, results, true);

            // Assert
            results.Should().HaveCount(1);
            results[0].ErrorMessage.Should().Contain("maximum length");
        }

        [Fact]
        public void Validate_WithNegativeTailLength_ShouldBeInvalid()
        {
            // Arrange
            var dto = new CreateDogDto
            {
                Name = "Buddy",
                Color = "brown",
                TailLength = -1,
                Weight = 25
            };

            // Act
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(dto, context, results, true);

            // Assert
            results.Should().HaveCount(1);
            results[0].ErrorMessage.Should().Contain("must be greater than or equal to 0");
        }
    }
}