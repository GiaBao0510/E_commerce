
using System.ComponentModel.DataAnnotations;

namespace E_commerce.Application.DTOs.Requests
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string phone_num {get; set;}
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string pass_word {get; set;}        
    }
}