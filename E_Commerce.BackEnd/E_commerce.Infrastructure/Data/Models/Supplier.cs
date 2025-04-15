using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Supplier
{
    public sbyte SupId { get; set; }

    public string SupName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNum { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string ContactPerson { get; set; } = null!;

    public string? Detail { get; set; }

    public string TaxCode { get; set; } = null!;

    public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();

    public virtual ICollection<PaymentSlip> PaymentSlips { get; set; } = new List<PaymentSlip>();
}
