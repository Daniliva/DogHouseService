using System.ComponentModel.DataAnnotations;

namespace DogHouse.DTOs
{
    public class CreateDogDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "TailLength must be greater than or equal to 0")]
        public int TailLength { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Weight must be greater than or equal to 0")]
        public int Weight { get; set; }
    }
}