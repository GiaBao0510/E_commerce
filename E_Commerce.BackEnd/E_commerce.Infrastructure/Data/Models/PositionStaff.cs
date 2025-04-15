using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class PositionStaff
{
    public int PositionId { get; set; }

    public string? PositionName { get; set; }

    public int? AllowanceCoefficient { get; set; }
}
