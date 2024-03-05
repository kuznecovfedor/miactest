using System;
using System.Collections.Generic;

namespace MIACData.Models;

public partial class Material
{
    public int IdMaterial { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int IdSeller { get; set; }

    public virtual Seller IdSellerNavigation { get; set; } = null!;
}
