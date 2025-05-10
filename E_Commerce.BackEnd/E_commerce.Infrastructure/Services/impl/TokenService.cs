using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using E_commerce.Application.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using E_commerce.Application.DTOs.Common;
using System.Text;
using E_commerce.Core.Exceptions;
using E_commerce.Infrastructure.Utils;
using Microsoft.Extensions.Options;
using E_commerce.Application.DTOs.Requests;
using CloudinaryDotNet;

namespace E_commerce.Infrastructure.Services.impl
{
    public class TokenService : ITokenService
    {
        #region ====[Private property]====
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SymmetricSecurityKey _secretKey;
        private readonly IRedisServices _redisServices;
        private readonly ILogger _logger;
        private readonly string _issuer;
        private readonly string _audience;
        #endregion

        ///<Summary>
        /// Hàm khởi tạo
        /// </Summary>
        public TokenService(
            IConfiguration configuration, 
            IUnitOfWork unitOfWork,
            ILogger logger,
            IRedisServices redisServices
        )
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _redisServices = redisServices;

            // Lấy khóa bí mật từ cấu hình
            _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:JWT:secret"])); 
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

        /// <summary>
        /// Hàm này sẽ kiểm tra accesstoken cũ đã hết hạn
        /// Mục đích là kiểm tra xem nguồn góc của token này có phải từ hệ thống cấp hay không
        /// </summary>
        /// <returns>True nếu token hợp lệ, false nếu không hợp lệ</returns>
        public async Task<string> _renewToken(TokenModelDTO token){

            //Đầu vào không được bỏ trống
            if(string.IsNullOrEmpty(token.accessToken) || string.IsNullOrEmpty(token.refreshToken))
                throw new ValidationException("Token không không được bỏ trống");
            
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var tokenValidateParam = new TokenValidationParameters{
                ValidateIssuer = false,           
                ValidateAudience = false,           
                ValidateIssuerSigningKey = true,    //Kiểm tra chữ ký
                ValidateLifetime = false,           //Vì kiểm tra token hết hạn. Nên tránh kiểm tra hạn sử dụng cảu nó
                IssuerSigningKey = _secretKey,      //Khóa bí mật
                ClockSkew = TimeSpan.Zero,          //Thời gian chênh lệch giữa máy chủ và máy khách
            };

            try{
                
                //check 1: Access token valid format
                var tokenInVerification = jwtTokenHandler.ValidateToken(
                    token.accessToken, 
                    tokenValidateParam,
                    out var validatedToken
                );

                //Check 2: Check algorithm - so sánh thuật toán mã hóa của token
                if(validatedToken is JwtSecurityToken jwtSecurityToken){
                    
                    var result = jwtSecurityToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha256Signature,
                        StringComparison.InvariantCultureIgnoreCase
                    );

                    if(!result)
                        throw new ValidationException("Thuật toán mã hóa token không hợp lệ");
                }

                //check 3: check accesstoken expired?
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault( x => 
                    x.Type == JwtRegisteredClaimNames.Exp).Value);
                
                var expireDate = CustomFormat.ConvertUnixTimeToDateTime(utcExpireDate);

                if(expireDate > DateTime.UtcNow)
                    throw new ValidationException("Token trên chưa hết hạn");
                
                //check 4: Check refresh token in white list and expired
                double score = await _redisServices.SortedSetGetScoreByValue("white_list", token.refreshToken);
                if(score == null || score < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                    throw new ValidationException("RefreshToken không tồn tại trong whitelist hoặc RefreshToken đã hết hạn");
                
                //Cấp token mới 
                BasicUserInfoDTO account = await _unitOfWork.users.GetBasicUserInfo(
                    tokenInVerification.Claims.FirstOrDefault(x => 
                        x.Type == ClaimTypes.NameIdentifier).Value
                );

                if(account == null)
                    throw new ValidationException("Không tìm thấy thông tin người dùng trong hệ thống");

                AccountInforDTO accountInforDTO = new AccountInforDTO{
                    user_id = account.user_id,
                    user_name = account.user_name,
                    email = account.email,
                    phone_num = account.phone_num
                };

                //Lưu access_token mới và xóa access_token cữ trong whitelist
                
                TokenDTO renew = await GenerateToken(accountInforDTO, 3);
                double accesstokenScore = CustomFormat.ConvertDateTimeToUnixTimestamp(renew.expiration);
                
                await Task.WhenAll(
                    _redisServices.SortedSetRemove("white_list", token.accessToken),
                    _redisServices.SortedSetAdd("white_list", accesstokenScore, renew.token)
                );

                return renew.token;
            }
            catch(Exception ex) when(!(ex is ECommerceException)){
                _logger.Error($"Lỗi khi kiểm tra nguồn gốc token: {ex.Message}", ex);
                throw new DetailsOfTheException(ex, "Lỗi khi kiểm tra nguồn gốc token");
            }
        }

        //Tạo token
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
                var roles = await _unitOfWork.users.ListOfRoleNames(accountInforDTO.user_id);
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

            }catch(Exception ex) when(!(ex is ECommerceException)){
                throw new DetailsOfTheException(ex,$"Lỗi khi tạo token");
            }
        }
    }
}

        //Thu hồi access token
        //public Task<bool> RevokeAccessToken(string token);

        //Thu hồi refresh token
        //public Task<bool> RevokeRefreshToken(string token);     