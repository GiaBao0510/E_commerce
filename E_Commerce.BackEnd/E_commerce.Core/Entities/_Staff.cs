namespace E_commerce.Core.Entities
{
    public class _Staff: _User
    {
        public string user_emp {get; set;} = "null";
        public string account_number {get; set;} = string.Empty;        
    }
}