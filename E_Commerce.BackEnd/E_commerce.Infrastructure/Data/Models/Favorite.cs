using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Favorite
{
    public string? UserClient { get; set; }

    public string? ProductId { get; set; }

    public virtual Good? Product { get; set; }

    public virtual Customer? UserClientNavigation { get; set; }
}
