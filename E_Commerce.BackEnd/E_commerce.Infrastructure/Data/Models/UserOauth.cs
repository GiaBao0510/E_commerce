using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class UserOauth
{
    public sbyte ProviderId { get; set; }

    public string UserId { get; set; } = null!;

    public string ExternalId { get; set; } = null!;

    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public virtual OauthProvider Provider { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
