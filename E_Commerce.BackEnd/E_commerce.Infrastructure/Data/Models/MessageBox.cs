using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class MessageBox
{
    public string? UserEmp { get; set; }

    public string? UserClient { get; set; }

    public string? Message { get; set; }

    public DateTime? Time { get; set; }

    public bool? Status { get; set; }

    public virtual Customer? UserClientNavigation { get; set; }

    public virtual Staff? UserEmpNavigation { get; set; }
}
