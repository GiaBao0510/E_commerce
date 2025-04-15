using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Collaborate
{
    public DateTime? WorkingTime { get; set; }

    public int PositionId { get; set; }

    public sbyte DepId { get; set; }

    public string? UserEmp { get; set; }

    public sbyte BranchId { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual Department Dep { get; set; } = null!;

    public virtual PositionStaff Position { get; set; } = null!;

    public virtual Staff? UserEmpNavigation { get; set; }
}
