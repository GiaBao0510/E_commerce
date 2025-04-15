using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Customer
{
    public string UserClient { get; set; } = null!;

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<ChatbotInteraction> ChatbotInteractions { get; set; } = new List<ChatbotInteraction>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual User UserClientNavigation { get; set; } = null!;
}
