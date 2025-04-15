using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class ChatbotInteraction
{
    public int ChatId { get; set; }

    public string Message { get; set; } = null!;

    public string Response { get; set; } = null!;

    public DateTime? Time { get; set; }

    public string? UserClient { get; set; }

    public string? UserEmp { get; set; }

    public virtual Customer? UserClientNavigation { get; set; }

    public virtual Staff? UserEmpNavigation { get; set; }
}
