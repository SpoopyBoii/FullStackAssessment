using System;
using System.Collections.Generic;

namespace FullStackAssessment.Api.Models;

public partial class Product
{
    public int Id { get; set; }

    public int ProductTypeId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public virtual ProductType ProductType { get; set; } = null!;
}
