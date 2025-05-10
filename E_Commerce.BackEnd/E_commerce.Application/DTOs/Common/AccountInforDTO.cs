namespace E_commerce.Application.DTOs.Common
{
    public class AccountInforDTO
    {
        public string user_id {get; set;}
        public string user_name {get; set;} = "null";
        public string phone_num {get; set;} = "null";
        public string email {get; set;} = "null";
        public string pass_word {get; set;} = "null";
        public int? is_block {get; set;}
        public int? is_delete {get; set;}
    }
}