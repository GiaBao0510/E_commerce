using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Comment
{
    public string? Comments { get; set; }

    public DateTime? Time { get; set; }

    public string? UserName { get; set; }

    public string Gmail { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public sbyte? TopicId { get; set; }

    public virtual Topic? Topic { get; set; }
}
