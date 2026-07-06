namespace FullStackAssessment.Api.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public int ProductTypeId { get; set; }
    public string ProductTypeName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
}