using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class WebSite
{
    public sbyte WebId { get; set; }

    public string? WebName { get; set; }

    public string? Url { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
