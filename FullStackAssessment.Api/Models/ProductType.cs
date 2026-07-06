using System;
using System.Collections.Generic;

namespace FullStackAssessment.Api.Models;

public partial class ProductType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
