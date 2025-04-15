using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class RolePermission
{
    public int RolePermissionId { get; set; }

    public sbyte RoleId { get; set; }

    public sbyte PermissionId { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
