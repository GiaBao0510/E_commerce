using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Permission
{
    public sbyte PermissionId { get; set; }

    public bool CanAdd { get; set; }

    public bool CanModify { get; set; }

    public bool CanDelete { get; set; }

    public bool CanView { get; set; }

    public sbyte WebId { get; set; }

    public sbyte DepId { get; set; }

    public string Resource { get; set; } = null!;

    public virtual Department Dep { get; set; } = null!;

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public virtual WebSite Web { get; set; } = null!;
}
