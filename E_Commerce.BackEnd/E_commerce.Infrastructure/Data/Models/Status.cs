using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Status
{
    public sbyte StatusId { get; set; }

    public string? StatusName { get; set; }

    public string? Detail { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
}
