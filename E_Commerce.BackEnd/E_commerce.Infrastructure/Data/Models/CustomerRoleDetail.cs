using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class CustomerRoleDetail
{
    public string UserClient { get; set; } = null!;

    public sbyte RankId { get; set; }

    public sbyte RoleId { get; set; }

    public virtual Rank Rank { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual Customer UserClientNavigation { get; set; } = null!;
}
