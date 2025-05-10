namespace E_commerce.Api.Authentication
{
    public static class CustomeAuthenticationHandler 
    {
        public static IApplicationBuilder UseTokenWhitelistValidation( this IApplicationBuilder builder){
            return builder.UseMiddleware<ValidateTokenInsideWhiteList>();
        }
    }
}