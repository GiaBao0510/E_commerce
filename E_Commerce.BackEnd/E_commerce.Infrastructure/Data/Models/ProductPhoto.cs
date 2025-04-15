using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class ProductPhoto
{
    public int ImgId { get; set; }

    public string ProductId { get; set; } = null!;

    public virtual Image Img { get; set; } = null!;

    public virtual Good Product { get; set; } = null!;
}
