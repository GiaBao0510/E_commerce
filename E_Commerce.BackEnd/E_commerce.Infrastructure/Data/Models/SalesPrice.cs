using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class SalesPrice
{
    public decimal Price { get; set; }

    public int NumOfProduct { get; set; }

    public DateTime? Time { get; set; }

    public string ProductId { get; set; } = null!;

    public int PromoId { get; set; }

    public virtual Good Product { get; set; } = null!;

    public virtual Promotion Promo { get; set; } = null!;
}
