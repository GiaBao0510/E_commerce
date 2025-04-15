using E_commerce.Application.Application;

namespace E_commerce.Infrastructure.Utils
{
    public class CustomFormat: ICustomFormat
    {
        //Định dạng lại ngày tháng năm
        public string FormatDateOfBirth(string dob){
            if(!string.IsNullOrEmpty(dob) && DateTime.TryParse(dob, out DateTime dateValue)){
               return  dateValue.ToString("yyyy-MM-dd");
            }
            return dob;
        }

        //Chuyển đổ số thực sang thời gian
        public DateTime ConvertUnixTimeToDateTime(long unixTime){
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            return dateTimeInterval.AddSeconds(unixTime).ToUniversalTime();
        }
    }
}