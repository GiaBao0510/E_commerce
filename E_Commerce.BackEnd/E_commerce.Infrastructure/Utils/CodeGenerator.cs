namespace E_commerce.Infrastructure.Utils
{
    public class CodeGenerator
    {
        private static readonly Random _random = new Random();
        private const string _letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string _numbers = "0123456789";

        static CodeGenerator(){ }

        /// <summary>
        ///Tạo một chuỗi số ngẫu nhiên 
        /// </summary>
        public static string GenerateRandomNumber(int length){
            return new string(Enumerable.Repeat(_numbers, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        ///Tạo một chuỗi ngẫu nhiên 
        /// </summary>
        public static string GenerateRandomString(int length){
            return new string(Enumerable.Repeat(_letters+_numbers, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static string GetTimePart(){
            DateTime now = DateTime.UtcNow;
            return now.ToString("HHmmss");
        }

        public static string GetDatePart(){
            DateTime now = DateTime.UtcNow;
            return now.ToString("yyMMdd");
        }

        public static string GenerateUID(){
            string TimePart = GetTimePart();
            string DatePart = GetDatePart();
            string RandomPart = GenerateRandomString(6);
            return DatePart + TimePart + RandomPart;
        }
    }
}