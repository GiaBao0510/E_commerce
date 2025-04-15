namespace E_commerce.Application.Application
{
    public interface ICodeGenerator
    {
        string GenerateRandomNumber(int length);
        string GenerateRandomString(int length);
        string GetTimePart();
        string GetDatePart();
        string GenerateUID();
    }
}