using System.ComponentModel.DataAnnotations;

namespace DogHouse.DTOs;

public class CreateDogDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Color { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int TailLength { get; set; }

    [Range(0, int.MaxValue)]
    public int Weight { get; set; }
}