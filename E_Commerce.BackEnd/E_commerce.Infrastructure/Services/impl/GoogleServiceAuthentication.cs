using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using E_commerce.Application.Application;
using E_commerce.Application.DTOs.Requests;
using E_commerce.Core.Entities;
using E_commerce.Core.Exceptions;
using E_commerce.Infrastructure.Data;
using E_commerce.SQL.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace E_commerce.Infrastructure.Services.impl
{
    public class GoogleServiceAuthentication: IGoogleServiceAuthentication
    {
        #region ===[Private Member]===
        private readonly DatabaseConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;            //HttpClient để gửi yêu cầu đến Google API
        private JsonWebKeySet _jsonWebKeySet;      //JsonWebKeySet chứa danh sách các khóa công khai (cache)
        private readonly object _lock = new object();       //Đối tượng khóa để đồng bộ hóa truy cập đến _jsonWebKeySet
        private DateTime _lastKeyFetch = DateTime.MinValue;      //Thời gian khóa công khai được lấy gần nhất
        private readonly ICustomerRepository _customerRepository;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        ///</summary>
        public GoogleServiceAuthentication(
            DatabaseConnectionFactory connectionFactory,
            ILogger logger,
            ICustomerRepository customerRepository,
            IConfiguration configuration,
            HttpClient httpClient
        ){
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jsonWebKeySet = new JsonWebKeySet();
        }

        /// <summary>
        /// Tìm thông tin người dùng thông qua email
        /// </summary>
        public async Task<_User> CheckVerifyAccountViaEmail(string email){
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync(
                    UserQueries.FindUserByEmail,
                    new { email }
                );

                return result;
            }
            catch(MySqlException ex){
                _logger.Error($"Database error when retrieving UserID: {email}, Error Number: {ex.Number}, Message:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error retrieving User with Email: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Xác thực Google ID token và trích xuất thông tin người dùng (email, name).
        /// </summary>
        /// <param name="dto">DTO chứa ID token từ Google.</param>
        /// <returns>Tuple chứa email và name. Phone không được trả về vì Google ID token hiếm khi chứa phone.</returns>
        /// <exception cref="SecurityTokenException">Ném khi token không hợp lệ.</exception>
        public async Task<(string?, string?)> VerifyGoogleToken(GoogleVerificationDTO googleVerificationDTO){
            try{
                
                //1.Xác thực token từ google
                var validationParameters = new TokenValidationParameters{
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _configuration["Authentication:Google:ValidIssuer"],
                    ValidAudience = _configuration["Authentication:Google:ValidAudience"],
                    IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                    {
                        JsonWebKeySet keysToReturn;

                        // Cache khóa công khai để tăng hiệu suất
                        lock (_lock)
                        {
                            if (_lastKeyFetch.AddHours(1) >= DateTime.UtcNow)
                            {
                                //trả về nếu cache chưa hết hạn
                                return _jsonWebKeySet.Keys;
                            }
                            keysToReturn = _jsonWebKeySet;
                        }

                        //Tài khoản mới bất đồng bộ ngoài lock
                        Task.Run(async ()=> {
                            var json = await _httpClient.GetStringAsync("https://www.googleapis.com/oauth2/v3/certs");
                            lock(_lock){
                                _jsonWebKeySet = JsonSerializer.Deserialize<JsonWebKeySet>(json) 
                                    ?? throw new JsonException("Không thể phân tích khóa công khai từ Google.");
                                _lastKeyFetch = DateTime.UtcNow; //Cập nhật thời gian khóa công khai được lấy gần nhất
                            }
                        }).GetAwaiter().GetResult();        //Chạy bất đồng bộ trong lambda không async

                        return keysToReturn.Keys;
                    }
                };

                // Xác thực token
                var handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(googleVerificationDTO.IdToken, validationParameters, out var validatedToken);

                //2. Trích xuất thông tin người dùng từ token
                var email =  principal.FindFirstValue(ClaimTypes.Email);
                var name =  principal.FindFirstValue(ClaimTypes.Name);

                return (email, name);
            }
            catch(SecurityTokenException ex){
                _logger.Error($"Lỗi khi xác thực token: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
            catch(HttpRequestException ex){
                _logger.Error($"Lỗi khi tải khóa công khai: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
            catch(JsonException ex){
                _logger.Error($"Lỗi khi phân tích khóa công khai: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Lỗi không xác định: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        /// <summary>
        /// Đăng nhập bằng google
        /// </summary>
        public async Task<(string, string)> Login(string name, string email){
            try{

                //1.Kiểm tra email người dùng có tồn tại trong tài khoảng không
                //Nếu chưa thì tạo mới tài khoản cho khách hàng
                //Nếu có thì 
            }

        }
    }
}