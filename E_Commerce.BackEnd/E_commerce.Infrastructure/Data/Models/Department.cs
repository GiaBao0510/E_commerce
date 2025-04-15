using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Department
{
    public sbyte DepId { get; set; }

    public string? DepName { get; set; }

    public string? Infor { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
