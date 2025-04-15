using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class GoodsReceipt
{
    public int PrId { get; set; }

    public DateTime? Time { get; set; }

    public decimal TotalImport { get; set; }

    public decimal TotalVat { get; set; }

    public decimal TotalValueOfReceipt { get; set; }

    public string Note { get; set; } = null!;

    public string UserEmp { get; set; } = null!;

    public sbyte SupId { get; set; }

    public int PsId { get; set; }

    public virtual PaymentSlip Ps { get; set; } = null!;

    public virtual Supplier Sup { get; set; } = null!;

    public virtual Staff UserEmpNavigation { get; set; } = null!;
}
