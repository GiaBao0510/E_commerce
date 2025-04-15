using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class ProductType
{
    public sbyte ProtyleId { get; set; }

    public string ProtyleName { get; set; } = null!;

    public string? AliasName { get; set; }

    public string? Details { get; set; }

    public virtual ICollection<Good> Goods { get; set; } = new List<Good>();
}
