namespace E_commerce.Core.Entities
{
    public class _Message
    {
        public string mess_id {get; set;} = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        public string text {get; set;}
        public DateTime send_date {get; set;} = DateTime.Now;
        public string from_number {get; set;} = string.Empty;
        public string conversation_id {get; set;} = string.Empty;
    }
}