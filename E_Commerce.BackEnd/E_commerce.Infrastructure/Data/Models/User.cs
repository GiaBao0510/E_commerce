using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Address { get; set; }

    public string? PhoneNum { get; set; }

    public string? Email { get; set; }

    public string? PassWord { get; set; }

    public bool? IsBlock { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<GroupChat> GroupChats { get; set; } = new List<GroupChat>();

    public virtual ICollection<LoginHistory> LoginHistories { get; set; } = new List<LoginHistory>();

    public virtual Staff? Staff { get; set; }
}
