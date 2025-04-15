using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Branch
{
    public sbyte BranchId { get; set; }

    public string BranchName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Describe { get; set; }
}
