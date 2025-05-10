using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Conversation
{
    public int ConversationId { get; set; }

    public string ConversationName { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual User User { get; set; } = null!;
}
