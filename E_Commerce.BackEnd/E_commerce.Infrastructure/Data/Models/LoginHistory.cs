using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class LoginHistory
{
    public int LoginId { get; set; }

    public string DeviceName { get; set; } = null!;

    public string? Detail { get; set; }

    public string Ipv4 { get; set; } = null!;

    public DateTime Time { get; set; }

    public string UserId { get; set; } = null!;

    public string? DeviceId { get; set; }

    public string? Location { get; set; }

    public string? OsName { get; set; }

    public string? Browser { get; set; }

    public DateTime? LoginTime { get; set; }

    public DateTime? LogoutTime { get; set; }

    public string LoginStatus { get; set; } = null!;

    public string LoginMethod { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
