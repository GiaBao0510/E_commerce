namespace E_commerce.Core.Entities
{
    public class _Conversation
    {
        public int conversation_id {get;  set;} = 0;
        public string? conversation_name {get;  set;} =  DateTime.Now.ToString("yyyyMMddHHmmss");
    }
}