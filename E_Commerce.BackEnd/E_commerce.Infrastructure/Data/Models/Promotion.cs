using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Promotion
{
    public int PromoId { get; set; }

    public string? PromoName { get; set; }

    public int? Discount { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }
}
