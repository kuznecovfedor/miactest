using System;
using System.Collections.Generic;

namespace MIACApi.Models;

public partial class User
{
    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
}
