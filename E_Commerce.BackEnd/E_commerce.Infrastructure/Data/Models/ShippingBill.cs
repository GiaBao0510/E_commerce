using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class ShippingBill
{
    public DateTime Time { get; set; }

    public decimal? Fee { get; set; }

    public string CurrentLocation { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public sbyte StatusId { get; set; }

    public string BillId { get; set; } = null!;

    public virtual Bill Bill { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
