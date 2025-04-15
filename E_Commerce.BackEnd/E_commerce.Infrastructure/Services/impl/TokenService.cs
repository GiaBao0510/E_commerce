using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using E_commerce.Application.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using E_commerce.Application.DTOs.Common;
using System.Text;
using E_commerce.Core.Exceptions;

namespace E_commerce.Infrastructure.Services.impl
{
    public class TokenService : ITokenService
    {
        #region ====[Private property]====
        private readonly IConfiguration _configuration;
        private readonly ICustomFormat _customFormat;
        private readonly IUserRepository _userRepository;
        private readonly SymmetricSecurityKey _secretKey;
        private readonly ILogger _logger;
        private readonly string _issuer;
        private readonly string _audience;

        #endregion

        ///<Summary>
        /// Hàm khởi tạo
        /// </Summary>
        public TokenService(
            IConfiguration configuration, 
            ICustomFormat customFormat, 
            IUserRepository userRepository,
            ILogger logger
        )
        {
            _configuration = configuration;
            _customFormat = customFormat;
            _userRepository = userRepository;
            _logger = logger;

            _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:JWT:secret"])); // Lấy khóa bí mật từ cấu hình
            _issuer = _configuration["Authentication:JWT:issuer"];
            _audience = _configuration["Authentication:JWT:audience"];
        }

        //Kiểm tra token còn hạn không
        public Task<bool> CheckIfTokenIsExpired(string token){
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var expirationDate = jwtToken.ValidTo;
                var currentDate = DateTime.UtcNow;
                
                // Trả về true nếu token còn hạn
                return Task.FromResult(expirationDate > currentDate);
            }
            catch(Exception ex)
            {
                _logger.Error($"Lỗi khi kiểm tra hạn token: {ex.Message}", ex);
                return Task.FromResult(false);
            }
        }

        //Kiểm tra xem token có hợp lệ không
        public async Task<bool> CheckIfTokenIsValid(string token){

            //Đầu vào không được bỏ trống
            if(string.IsNullOrEmpty(token)){
                _logger.Error("Token không không được bỏ trống");
                throw new DetailsOfTheException(new Exception("Token không hợp lệ"), "Token không không được bỏ trống");
            }
            
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            //Kiểm tra có thể đọc được token này không
            if(!jwtTokenHandler.CanReadToken(token)){
                _logger.Error("Token không hợp lệ");
                throw new DetailsOfTheException(new Exception("Token không hợp lệ"), "Token không hợp lệ");
            }

            //0.Đọc thông tin từ token
            var tokenData = jwtTokenHandler.ReadJwtToken(token);

            var tokenValidParametes = new TokenValidationParameters{
                
                //Tự cấp token
                ValidateIssuer = false,     //Không kiểm tra người phát hành token 
                ValidateAudience = false,   //Không kiểm tra đối tượng nhận token
                ValidateLifetime = false,   //Không kiểm tra hết hạn của token. Vì đây là giai hạn lại toekn. Nếu không khai thuộc tính này có giá trị false thì nó sẽ báo lỗi

                //Ký vào token
                ValidateIssuerSigningKey = true,    // kiểm tra khóa bí mật đã được sử dụng ký vào token, đảm bảo việc token không bị thay đổi
                IssuerSigningKey = _secretKey,      // Khóa bí mật được sử dụng ở dạng byte sẽ được sử dụng để xác thực token, đây cũng là khóa sử dụng để ký xác thực
                ClockSkew = TimeSpan.Zero           // Đặt độ lệch thời gian xác thực token là 0. 
            }; 

            //1. Kiểm tra valid format token
            var tokenInverification = jwtTokenHandler.ValidateToken(
                token, 
                tokenValidParametes,
                out var validatedToken
            );

            //2. Kiểm tra thuật toán mã hóa. Nếu không đúng thuật toán mã hóa thì trả về ngoại lệ
            if(validatedToken is JwtSecurityToken jwtSecurityToken){
                var result = jwtSecurityToken.Header        //So sánh kiểm tra có đúng thuật toán này có dùng để mã hóa khồng
                    .Alg.Equals("http://www.w3.org/2001/04/xmldsig-more#hmac-sha256", 
                        StringComparison.InvariantCultureIgnoreCase);

                if(!result){
                    _logger.Error("Thuật toán mã khóa token không hợp lệ");
                    throw new DetailsOfTheException(new Exception("Token không hợp lệ"), "Thuật toán mã khóa token không hợp lệ");
                }
            }
            
            //3. Kiểm tra token còn hạn không
            var utcExpireDate = long.Parse(tokenInverification.Claims.FirstOrDefault(
                x => x.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Exp
            ).Value);

            var expireDate = _customFormat.ConvertUnixTimeToDateTime(utcExpireDate);
            
            if(expireDate < DateTime.UtcNow){
                _logger.Error("Token đã hết hạn");
                throw new DetailsOfTheException(new Exception("Token đã hết hạn"), "Token đã hết hạn");
            }
            
            return true;
        }

        //Tạo Access token
        public async Task<TokenDTO> GenerateToken(AccountInforDTO accountInforDTO, int time = 24){
            try{
                var creds = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256Signature); // Tạo chữ ký cho token
            
                //Chỉ định các Claims cho người dùng
                var claims = new List<Claim>(){
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
                    new Claim(ClaimTypes.NameIdentifier, accountInforDTO.user_id ), 
                    new Claim(ClaimTypes.Name, accountInforDTO.user_name),
                    new Claim(ClaimTypes.Email, accountInforDTO.email), 
                    new Claim(ClaimTypes.MobilePhone ,accountInforDTO.phone_num ), 
                };

                //Lấy danh sách vai trò của người dùng
                var roles = await _userRepository.ListOfRoleNames(accountInforDTO.user_id);
                foreach(var role in roles){
                    claims.Add(new Claim(ClaimTypes.Role, role.role_name));
                }

                //Tạo token dựa trên thời gian
                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(time), //Thời gian hết hạn của token
                    signingCredentials: creds                //Chữ ký của token
                );

                var handler = new JwtSecurityTokenHandler();

                return new TokenDTO{
                    token = handler.WriteToken(token),  //Chuyển đổi token thành chuỗi
                    expiration = token.ValidTo,         //Thời gian hết hạn của token
                    UID = accountInforDTO.user_id       //ID của người dùng
                };

            }catch(Exception ex){
                throw new DetailsOfTheException(ex,$"Lỗi khi tạo token");
            }
        }

        //Thu hồi access token
        //public Task<bool> RevokeAccessToken(string token);

        //Thu hồi refresh token
        //public Task<bool> RevokeRefreshToken(string token);        
    }
}