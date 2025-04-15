namespace E_commerce.Core.Entities
{
    public class _Rank
    {
        public sbyte rank_id { get; set; } = 0;
        public string? rank_name {get; set;}
        public int rating_point { get; set; } = 0;
        public string describe { get; set; } = "Null";
    }
}