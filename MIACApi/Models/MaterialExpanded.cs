using MIACApi.DTO;
using System;
using System.Collections.Generic;

namespace MIACApi.Models;

public partial class Material
{
    public MaterialDTO ToDTO() {
        return new MaterialDTO 
        {
            IdMaterial = this.IdMaterial,
            Name = this.Name,
            Price = this.Price,
            IdSeller = this.IdSeller
        };
    }
}
