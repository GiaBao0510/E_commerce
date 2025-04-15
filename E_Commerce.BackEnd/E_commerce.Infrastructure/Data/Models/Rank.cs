using System;
using System.Collections.Generic;

namespace E_commerce.Infrastructure.Data.Models;

public partial class Rank
{
    public sbyte RankId { get; set; }

    public string RankName { get; set; } = null!;

    public int RatingPoint { get; set; }

    public string? Describe { get; set; }
}
