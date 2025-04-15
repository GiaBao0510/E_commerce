using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class SupplierLogo
{
    public int ImgId { get; set; }

    public sbyte SupId { get; set; }

    public virtual Image Img { get; set; } = null!;

    public virtual Supplier Sup { get; set; } = null!;
}
