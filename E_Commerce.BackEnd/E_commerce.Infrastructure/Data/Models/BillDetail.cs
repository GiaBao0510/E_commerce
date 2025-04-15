using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class BillDetail
{
    public int? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public string BillId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public virtual Bill Bill { get; set; } = null!;

    public virtual Good Product { get; set; } = null!;
}
