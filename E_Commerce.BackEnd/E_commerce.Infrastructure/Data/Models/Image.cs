using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Image
{
    public int ImgId { get; set; }

    public string PublicId { get; set; } = null!;

    public string? PathImg { get; set; }
}
