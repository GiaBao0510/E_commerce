using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Good
{
    public string ProductId { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string? Details { get; set; }

    public string? Characteristic { get; set; }

    public int? NumOfView { get; set; }

    public sbyte? ProtyleId { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ProductType? Protyle { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
