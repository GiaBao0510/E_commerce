using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Review
{
    public int Id { get; set; }

    public string UserClient { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public int? Rating { get; set; }

    public string? Note { get; set; }

    public DateTime? Time { get; set; }

    public virtual Good Product { get; set; } = null!;

    public virtual Customer UserClientNavigation { get; set; } = null!;
}
