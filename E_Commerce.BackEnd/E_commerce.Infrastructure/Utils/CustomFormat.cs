namespace E_commerce.Infrastructure.Utils
{
    public static class CustomFormat
    {
        //Hàm khởi tạo
        static CustomFormat(){}

        //Định dạng lại ngày tháng năm
        public static string FormatDateOfBirth(string dob){
            if(!string.IsNullOrEmpty(dob) && DateTime.TryParse(dob, out DateTime dateValue)){
               return  dateValue.ToString("yyyy-MM-dd");
            }
            return dob;
        }

        //Chuyển đổ số thực sang thời gian
        public static DateTime ConvertUnixTimeToDateTime(long unixTime){
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTimeInterval.AddSeconds(unixTime).ToUniversalTime();
        }

        //Chuyeeeer đổi datetime sang double
        public static double ConvertDateTimeToUnixTimestamp(DateTime datetime){
            
            //Khởi tạo mốc thời gian từ Unix Epoch (1/1/1970 00:00:00)
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            //Trả về số giây từ thời điểm UnixEpoch đến thời điểm muốn chuyển đổi
            return (datetime.ToUniversalTime() - unixEpoch).TotalSeconds;
        }
    }
}