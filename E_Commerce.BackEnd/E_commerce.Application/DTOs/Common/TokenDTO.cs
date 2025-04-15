namespace E_commerce.Application.DTOs.Common
{
    public class TokenDTO
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
        public string UID { get; set; }
    }
}