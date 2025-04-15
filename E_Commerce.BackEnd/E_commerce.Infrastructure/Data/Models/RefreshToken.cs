using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class RefreshToken
{
    public string UserId { get; set; } = null!;

    public string? Token { get; set; }

    public string? Jwt { get; set; }

    public bool? IsUse { get; set; }

    public bool? IsRevoked { get; set; }

    public DateTime? IsSureAt { get; set; }

    public DateTime? Expired { get; set; }

    public virtual User User { get; set; } = null!;
}
