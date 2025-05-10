using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Message
{
    public string MessId { get; set; } = null!;

    public string Text { get; set; } = null!;

    public DateTime? SendDate { get; set; }

    public string FromNumber { get; set; } = null!;

    public int ConversationId { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;
}
