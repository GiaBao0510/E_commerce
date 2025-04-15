using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class ProductTypeDetail
{
    public sbyte ProtyleId { get; set; }

    public sbyte SupId { get; set; }

    public virtual ProductType Protyle { get; set; } = null!;

    public virtual Supplier Sup { get; set; } = null!;
}
