using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Topic
{
    public sbyte TopicId { get; set; }

    public string? TopicName { get; set; }

    public string? UserEmp { get; set; }

    public virtual Staff? UserEmpNavigation { get; set; }
}
