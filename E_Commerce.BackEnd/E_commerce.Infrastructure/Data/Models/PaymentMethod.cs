using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class PaymentMethod
{
    public sbyte PmtId { get; set; }

    public string? PmtName { get; set; }

    public string? AccountNumber { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
}
