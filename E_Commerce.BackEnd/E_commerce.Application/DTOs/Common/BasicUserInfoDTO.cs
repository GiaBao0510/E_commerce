namespace E_commerce.Application.DTOs.Common
{
    public class BasicUserInfoDTO
    {
        public string user_id {get; set;}
        public string user_name {get; set;} = "null";
        public string phone_num {get; set;} = "null";
        public string email {get; set;} = "null";
        public List<string> roles {get; set;}

    }
}