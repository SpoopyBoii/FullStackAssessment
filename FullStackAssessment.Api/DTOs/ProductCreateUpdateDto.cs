using System.ComponentModel.DataAnnotations;

namespace FullStackAssessment.Api.DTOs;

public class ProductCreateUpdateDto
{
    [Required]
    public int ProductTypeId { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Required]
    [Range(0.00, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
    public decimal Price { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
}