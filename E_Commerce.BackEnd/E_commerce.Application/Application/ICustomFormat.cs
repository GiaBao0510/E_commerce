namespace E_commerce.Application.Application
{
    public interface ICustomFormat
    {
        public string FormatDateOfBirth(string dob);
        public DateTime ConvertUnixTimeToDateTime(long unixTime);
    }
}