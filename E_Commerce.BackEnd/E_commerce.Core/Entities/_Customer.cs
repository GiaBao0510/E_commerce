namespace E_commerce.Core.Entities
{
    public class _Customer : _User
    {
        public string user_client {get; set;} = "null";
        public int? rank_id {get; set;}
        public string? rank_name {get; set;}
        public int? rating_point {get; set;} 
    }
}