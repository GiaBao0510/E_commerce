namespace E_commerce.Core.Entities
{
    public class _User
    {
        public string user_id {get; set;} = "null";
        public string? user_name {get; set;}
        public string? date_of_birth {get; set;} 
        public string? address {get; set;}
        public string? phone_num {get; set;}
        public string? email {get; set;}
        public string? pass_word {get; set;}
        public bool? is_block {get; set;}
        public bool? is_delete {get; set;}
    }
}