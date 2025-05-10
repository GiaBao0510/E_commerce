namespace E_commerce.Application.DTOs.Requests
{
    public class TokenModelDTO
    {
        public string accessToken { get; set; } = string.Empty;
        public string refreshToken { get; set; } = string.Empty;
    }
}