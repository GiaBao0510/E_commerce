using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace E_commerce.Infrastructure.Payments.Vnpay
{
    public class VnPayLibrary
    {
        public const string VERSION = "2.1.0";
        private SortedList<string, string> _requestData = 
            new SortedList<string, string>(new VnPayCompare());
        private SortedList<string, string> _responsetData = 
            new SortedList<string, string>(new VnPayCompare());
        
        public void AddRequestData(string key, string value){
            if(!String.IsNullOrEmpty(value))
                _requestData.Add(key, value);
        }

        public void AddResponseData(string key, string value){
            if(!String.IsNullOrEmpty(value))
                _responsetData.Add(key, value);
        }

        public string GetResponseData(string key){
            string value;
            if(_responsetData.TryGetValue(key, out value))
                return value;
            return string.Empty;
        }

        #region  Request

        /// <summary>
        /// Mục đích: hàm tạo một URL yêu cầu thanh toán cho VnPay
        /// <br/>
        /// Đầu vào:
        /// <br/>
        /// - baseUrl: URL cơ sở của VnPay
        /// <br/>
        /// - vnp_HashSecrete: khóa bí mật của VnPay
        ///  <br/>
        /// Đầu ra: Một chuỗi URL yêu cầu thanh toán đã được mã hóa
        /// <br/>
        /// Lưu ý:
        /// - Hàm này sẽ mã hóa các tham số yêu cầu và tạo mã băm HMAC-SHA512
        /// - Các tham số yêu cầu được lưu trữ trong biến _requestData
        /// - Hàm này sẽ thêm mã băm vào URL yêu cầu
        /// - Hàm này sẽ trả về một chuỗi URL yêu cầu thanh toán đã được mã hóa
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="vnp_HashSecrete"></param>
        public string CreateRequestUrl(string baseUrl, string vnp_HashSecrete)
        {
            StringBuilder data = new StringBuilder();
            foreach( KeyValuePair<string, string> kv in _requestData){
                
                if(!String.IsNullOrEmpty(kv.Value))
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }

            string queryString = data.ToString();
            baseUrl += "?" + queryString;
            String singData = queryString;
            
            if(singData.Length > 0)
                singData = singData.Remove(data.Length - 1, 1);

            string vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecrete, singData );
            baseUrl += "vnp_SecureHash=" + vnp_SecureHash;
            return baseUrl;
        }
        #endregion

        #region  Response Process
        public bool ValidateSignature(string inputHash, string secretKey){
            string rspRaw = GetResponseData();
            string myChecksum = myChecksum = Utils.HmacSHA512(secretKey, rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        public string GetResponseData(){
            
            StringBuilder data = new StringBuilder();

            if(_responsetData.ContainsKey("vnp_SecureHashType"))
                _responsetData.Remove("vnp_SecureHashType");
            
            if(_responsetData.ContainsKey("vnp_SecureHash"))
                _responsetData.Remove("vnp_SecureHash");
            

            foreach(KeyValuePair<string, string> kv in _responsetData){
                if(!String.IsNullOrEmpty(kv.Value))
                    data.Append(WebUtility.UrlEncode(kv.Key)  + "=" +  WebUtility.UrlEncode(kv.Value) + "&");
            }

            //Remove last '&'
            if(data.Length > 0)
                data.Remove(data.Length - 1, 1);

            return data.ToString();
        }
        #endregion
    }


    /// <summary>
    /// Lớp tiện ích
    /// </summary>
    public class Utils{

        /// <summary>
        /// Mục đích: hàm tạo một mã băm SHA512 từ inputData sử dụng thuật toán HMAC-SHA512 với một khóa bí mật key.
        /// <br/>
        /// Đầu vào:
        /// <br/>
        /// - key: chuỗi khóa bí mật để tạo mã băm. Chỉ những ai biết khóa này mới có thể tạo ra mã băm giống như mã băm đã được tạo ra.
        /// <br/>
        /// - inputData: chuỗi dữ liệu đầu vào cần mã hóa.
        /// <br/>
        /// Đầu ra: Một chuỗi hex (dạng chữ số thập phận) biểu diễn mã băm
        /// </summary>
        /// <param name="key"></param>
        /// <param name="inputData"></param>
        public static string HmacSHA512(string key, String inputData){
            
            var hash = new StringBuilder();     //Dùng StringBuilder tối ưu hơn trong việc nối nhiều đoạn

            //Chuyển chuỗi thành mảng byte
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);

            //Tạo một đối tượng HMACSHA512 với khóa bí mật
            using(var hmac = new HMACSHA512(keyBytes)){
                //Tính toán mã băm
                byte[] hashValue = hmac.ComputeHash(inputBytes);

                //Chuyển đổi mảng byte thành chuỗi hex
                foreach(byte b in hashValue){
                    hash.Append(b.ToString("x2"));
                }
            }

            return hash.ToString(); //Trả về chuỗi hex
        }

        public static string GetIpAddress(HttpContext context){
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;
                if(remoteIpAddress != null){
                    if(remoteIpAddress.AddressFamily == AddressFamily.InterNetwork){
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    if(remoteIpAddress != null)
                        ipAddress = remoteIpAddress.ToString();
                    return ipAddress;
                }
            }
            catch(Exception ex){
                return "Invalid IP:" + ex.Message;
            }
            return "127.0.0.1";
        }
    }

    /// <summary>
    /// VnPayCompare lớp này dùng để so sánh các chuỗi trong danh sách
    /// </summary>
    public class VnPayCompare: IComparer<string>{
        public int Compare(string x, string y){
            if( x == y) return 0;
            if(x == null) return -1;
            if(y == null) return 1;
            var vnCompare = CompareInfo.GetCompareInfo("en-US");
            return  vnCompare.Compare(x,y, CompareOptions.Ordinal);
        }
    }
}