using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Cart
{
    public DateTime Time { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public string UserClient { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public int CartId { get; set; }

    public virtual Good Product { get; set; } = null!;

    public virtual Customer UserClientNavigation { get; set; } = null!;
}
