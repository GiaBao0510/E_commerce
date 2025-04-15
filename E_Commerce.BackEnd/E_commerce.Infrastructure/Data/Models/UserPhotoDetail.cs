using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class UserPhotoDetail
{
    public string UserId { get; set; } = null!;

    public int ImgId { get; set; }

    public virtual Image Img { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
