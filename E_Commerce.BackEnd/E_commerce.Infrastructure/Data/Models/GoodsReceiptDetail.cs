using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class GoodsReceiptDetail
{
    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public int PrId { get; set; }

    public string ProductId { get; set; } = null!;

    public virtual GoodsReceipt Pr { get; set; } = null!;

    public virtual Good Product { get; set; } = null!;
}
