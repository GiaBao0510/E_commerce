using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class StaffRoleDetail
{
    public string UserEmp { get; set; } = null!;

    public string? Describe { get; set; }

    public sbyte RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual Staff UserEmpNavigation { get; set; } = null!;
}
