using Dapper;
using E_commerce.Application.Application;
using E_commerce.Application.DTOs.Common;
using E_commerce.Application.DTOs.Requests;
using E_commerce.Core.Exceptions;
using E_commerce.Infrastructure.Data;
using E_commerce.SQL.Queries;
using MySql.Data.MySqlClient;

namespace E_commerce.Infrastructure.Services.impl
{
    public class AccountServices: IAccountServices
    {
        #region ======[Private property]=====
        private readonly ILogger _logger;
        private readonly DatabaseConnectionFactory _connectionFactory;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ITokenListService _tokenListService;
        #endregion

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public AccountServices(
            ILogger logger,
            DatabaseConnectionFactory connectionFactory,
            IUserRepository userRepository,
            ITokenService tokenService,
            ITokenListService tokenListService
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _tokenListService = tokenListService ?? throw new ArgumentNullException(nameof(tokenListService));
        }
        
        //Kiểm tra tài khoản có bị khóa không
        public async Task<bool> CheckIfAccountIsBlocked(string phone_num){
            try{
                using var connection = _connectionFactory.CreateConnection();
                
                var result = await connection.ExecuteScalarAsync<int>(
                    AccountQueries.CheckIfAccountIsBlocked,
                    new { phone_num = phone_num }
                );

                return result > 0;
            }
            catch(MySqlException ex){
                _logger.Error($"Database error when Check If Account Is Blocked: {phone_num}, Error Number: {ex.Number}, Message:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error wher check if account is blocked: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        //Kiểm tra tài khoản có bị xóa không
        public async Task<bool> CheckIfAccountIsDeleted(string phone_num){
            try{
                using var connection = _connectionFactory.CreateConnection();
                
                var result = await connection.ExecuteScalarAsync<int>(
                    AccountQueries.CheckIfAccountIsBlocked,
                    new { phone_num = phone_num }
                );

                return result > 0;
            }
            catch(MySqlException ex){
                _logger.Error($"Database error when Check If Account Is Blocked: {phone_num}, Error Number: {ex.Number}, Message:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error when check if account is blocked: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        //Đăng nhập tài khoản
        public async Task<(TokenDTO,TokenDTO)> Login(LoginDTO login){
            try{

                //Kiểm tra đầu vào
                if(string.IsNullOrEmpty(login.phone_num) || string.IsNullOrEmpty(login.pass_word))
                    throw new ValidationException("Tài khoản hoặc mật khẩu không được để trống!");
                
                //Kiểm tra tài khoản có bị khóa không
                if(await CheckIfAccountIsBlocked(login.phone_num))
                    throw new ResourceNotFoundException($"Tài khoản {login.phone_num} đã bị khóa!");

                
                //Kiểm tra tài khoản có bị xóa không
                if(await CheckIfAccountIsDeleted(login.phone_num))
                    throw new ResourceNotFoundException($"Tài khoản {login.phone_num} đã bị xóa!");

                //Kiểm tra lấy số điện thoại, mật khẩu đã băm và UID người dùng dựa trên thông tin đầu vào
                using var connection = _connectionFactory.CreateConnection();
                AccountInforDTO result = await connection.QueryFirstOrDefaultAsync<AccountInforDTO>(
                    AccountQueries.CheckIfAccountExists,
                    new {login.phone_num }
                );

                //Kiểm tra null
                if(result == null)
                    throw new ResourceNotFoundException($"Tài khoản {login.phone_num} không tồn tại!");

                //Kiểm tra mật khẩu có trùng khớp không
                bool isPwdCorrect = BCrypt.Net.BCrypt.Verify( login.pass_word, result.pass_word);
                if(!isPwdCorrect)
                    throw new ValidationException("Mật khẩu không đúng!");
                
                //Tạo access token & refreshtoken dựa trên người dùng
                TokenDTO accesstoken = await _tokenService.GenerateToken(result, 3);
                TokenDTO refreshtoken = await _tokenService.GenerateToken(result, 24 * 30); //Thời gian hết hạn là 30 ngày

                double expiration = (refreshtoken.expiration - DateTime.UnixEpoch).TotalSeconds; //Thời gian hết hạn của refreshtoken 

                //Lưu refresh token vào trong whiteList
                bool ketqua = await _tokenListService.AddTokenToSortedSetWithMapping( result.user_id ,refreshtoken.token, expiration , "white_list");
                 
                //Chỉ gửi accessToken 
                return (accesstoken, refreshtoken);
                
            }catch(MySqlException ex){
                _logger.Error($"Database error when login:  Error Number: {ex.Number}, Message:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error when login: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
        
        //Đăng ký
        //Đăng xuất
        //Lấy thông tin tài khoản
        
        // Xóa tài khoản
        public async Task<bool> DeleteAccount(string user_id){
            try{
                
                //Kiểm tra tài khoản có tồn tại không
                await _userRepository.IsUserIdExists(user_id);

                using var connection = _connectionFactory.CreateConnection();
                
                var result = await connection.ExecuteScalarAsync<int>(
                    AccountQueries.DeleteAccount,
                    new { user_id }
                );

                return result > 0;
            }
            catch(MySqlException ex){
                _logger.Error($"Database error when Delete Account: {user_id}, Error Number: {ex.Number}, Message:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error wher delete account: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        //Khóa tài khoản
        public async Task<bool> BlockAccount(string user_id){
            try{
                
                //Kiểm tra tài khoản có tồn tại không
                await _userRepository.IsUserIdExists(user_id);

                //Lấy refreshtoken từ ánh xạ
                var refreshToken = await _tokenListService.GetTokenByUID(user_id);
                if(!string.IsNullOrEmpty(refreshToken)){
                    
                    //Kiểm tra refresh token có tồn tại trong whiteList không
                    var score = await _tokenListService.IsTokenInSortedSet(refreshToken);
                    if(score != -1){

                        //Xóa refresh token trong whiteList
                        await _tokenListService.DeleteTokenFromSortedSet(refreshToken, "white_list");
                        
                        //Thêm token vào trong blacklist
                        await _tokenListService.AddTokenToSortedSetWithMapping(user_id, refreshToken, score, "black_list");
                    }
                }

                using var connection = _connectionFactory.CreateConnection();
                
                var result = await connection.ExecuteScalarAsync<int>(
                    AccountQueries.BlockAccount,
                    new { user_id }
                );

                return result > 0;
            }
            catch(MySqlException ex){
                _logger.Error($"Database error when Delete Account: {user_id}, Error Number: {ex.Number}, Message:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error wher delete account: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        //Khôi phục tài khoản
        public async Task<bool> RecoverAccount(string user_id){
            try{
                
                //Kiểm tra tài khoản có tồn tại không
                await _userRepository.IsUserIdExists(user_id);

                using var connection = _connectionFactory.CreateConnection();
                
                var result = await connection.ExecuteScalarAsync<int>(
                    AccountQueries.RecoverAccount,
                    new { user_id }
                );

                return result > 0;
            }
            catch(MySqlException ex){
                _logger.Error($"Database error when Delete Account: {user_id}, Error Number: {ex.Number}, Message:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error wher delete account: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        //Mở khóa tài khoản
        public async Task<bool> UnlockAccount(string user_id){
            try{
                
                //Kiểm tra tài khoản có tồn tại không
                await _userRepository.IsUserIdExists(user_id);

                using var connection = _connectionFactory.CreateConnection();
                
                var result = await connection.ExecuteScalarAsync<int>(
                    AccountQueries.UnlockAccount,
                    new { user_id }
                );
 
                return result > 0;
            }
            catch(MySqlException ex){
                _logger.Error($"Database error when Delete Account: {user_id}, Error Number: {ex.Number}, Message:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error wher delete account: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }

        //Thay đổi mật khẩu
        public async Task<bool> ChangePassword(string user_id, ChangePasswordDTO changePasswordDTO){
            try{
                
                //Lấy mật khẩu đã băm dựa trên ID người dùng
                string HashPassword = await _userRepository.GetHashedPasswordByUserID(user_id);

                //Kiểm tra đầu vào không được rỗng
                if(string.IsNullOrEmpty(changePasswordDTO.old_pwd) || string.IsNullOrEmpty(changePasswordDTO.new_pwd))
                    throw new ValidationException("Mật khẩu không được để trống!");

                //Kiểm tra mật khẩu cũ có đúng không
                bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify( changePasswordDTO.old_pwd, HashPassword);

                if(!isPasswordCorrect)
                    throw new ValidationException("Mật khẩu cũ không đúng!");
                
                //Cập nhật lại mật khẩu mới
                HashPassword = BCrypt.Net.BCrypt.HashPassword(changePasswordDTO.new_pwd);
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteScalarAsync<int>(
                    AccountQueries.ChangePassword,
                    new { 
                        user_id, 
                        new_pass_word = HashPassword 
                    } 
                );

                return result > 0;
            }catch(MySqlException ex){
                _logger.Error($"Database error when Delete Account: {user_id}, Error Number: {ex.Number}, Message:{ex.Message}", ex);
                throw new DetailsOfTheMysqlException(ex);
            }
            catch(Exception ex) when (!(ex is ECommerceException)){
                _logger.Error($"Error wher delete account: {ex.Message}", ex);
                throw new DetailsOfTheException(ex);
            }
        }
    }
}