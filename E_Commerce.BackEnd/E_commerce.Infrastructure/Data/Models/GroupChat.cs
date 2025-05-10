using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class GroupChat
{
    public int GroupId { get; set; }

    public string GroupName { get; set; } = null!;

    public bool? GroupType { get; set; }

    public DateTime? JoinDate { get; set; }

    public string UserId { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
