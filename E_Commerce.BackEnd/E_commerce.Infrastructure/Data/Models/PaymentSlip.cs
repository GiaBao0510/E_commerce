using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class PaymentSlip
{
    public int PsId { get; set; }

    public DateTime? Time { get; set; }

    public decimal PaymentAmount { get; set; }

    public string Note { get; set; } = null!;

    public string UserEmp { get; set; } = null!;

    public sbyte SupId { get; set; }

    public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();

    public virtual Supplier Sup { get; set; } = null!;

    public virtual Staff UserEmpNavigation { get; set; } = null!;
}
