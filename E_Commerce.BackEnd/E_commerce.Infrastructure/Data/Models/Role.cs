using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Role
{
    public sbyte RoleId { get; set; }

    public string? RoleName { get; set; }

    public string? Describe { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
