using E_commerce.Application.Application;

namespace E_commerce.Infrastructure.Utils
{
    public class CodeGenerator: ICodeGenerator
    {
        private static readonly Random _random = new Random();
        private const string _letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string _numbers = "0123456789";

        /// <summary>
        ///Tạo một chuỗi số ngẫu nhiên 
        /// </summary>
        public string GenerateRandomNumber(int length){
            return new string(Enumerable.Repeat(_numbers, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        ///Tạo một chuỗi ngẫu nhiên 
        /// </summary>
        public string GenerateRandomString(int length){
            return new string(Enumerable.Repeat(_letters+_numbers, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public string GetTimePart(){
            DateTime now = DateTime.UtcNow;
            return now.ToString("HHmmss");
        }

        public string GetDatePart(){
            DateTime now = DateTime.UtcNow;
            return now.ToString("yyMMdd");
        }

        public string GenerateUID(){
            string TimePart = GetTimePart();
            string DatePart = GetDatePart();
            string RandomPart = GenerateRandomString(6);
            return DatePart + TimePart + RandomPart;
        }
    }
}