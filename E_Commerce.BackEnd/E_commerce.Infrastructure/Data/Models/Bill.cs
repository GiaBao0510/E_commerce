using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Bill
{
    public string BillId { get; set; } = null!;

    public DateTime? InvoiceDate { get; set; }

    public decimal? ShippingFee { get; set; }

    public string? Note { get; set; }

    public string UserClient { get; set; } = null!;

    public sbyte PmtId { get; set; }

    public sbyte StatusId { get; set; }

    public string? UserEmp { get; set; }

    public virtual PaymentMethod Pmt { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;

    public virtual Customer UserClientNavigation { get; set; } = null!;

    public virtual Staff? UserEmpNavigation { get; set; }
}
