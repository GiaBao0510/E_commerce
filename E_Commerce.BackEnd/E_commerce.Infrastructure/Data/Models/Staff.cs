using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Staff
{
    public string UserEmp { get; set; } = null!;

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<ChatbotInteraction> ChatbotInteractions { get; set; } = new List<ChatbotInteraction>();

    public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();

    public virtual ICollection<PaymentSlip> PaymentSlips { get; set; } = new List<PaymentSlip>();

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();

    public virtual User UserEmpNavigation { get; set; } = null!;
}
