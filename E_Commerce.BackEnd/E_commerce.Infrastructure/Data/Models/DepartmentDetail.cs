using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class DepartmentDetail
{
    public DateTime? TimeOfCreate { get; set; }

    public string? Describe { get; set; }

    public sbyte BranchId { get; set; }

    public sbyte DepId { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual Department Dep { get; set; } = null!;
}
