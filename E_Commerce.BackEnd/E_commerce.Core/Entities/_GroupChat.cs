
namespace E_commerce.Core.Entities
{
    public class _GroupChat
    {
        public int group_id { get; set; }
        public string group_name { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmss");
        public bool is_group {get; set;} = false;
        public DateTime join_date { get; set; } = DateTime.Now;
        public string user_id { get; set; }
        public string conversation_id { get; set; } 
    }
}