using System;
using System.Collections.Generic;

namespace MIACApi.Models;

public partial class Seller
{
    public int IdSeller { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string? Patronymic { get; set; }

    public DateOnly RegistrationDate { get; set; }

    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
