namespace E_commerce.Application.DTOs.Requests
{
    public class ChangePasswordDTO
    {
        public string old_pwd { get; set; }
        public string new_pwd { get; set; }        
    }
}