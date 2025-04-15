using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class OauthProvider
{
    public sbyte ProviderId { get; set; }

    public string OauthName { get; set; } = null!;

    public string ClientId { get; set; } = null!;

    public string ClientSecret { get; set; } = null!;

    public bool? Enabled { get; set; }
}
