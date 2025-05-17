# Tích hợp Google OAuth 2.0 với .NET (Backend) và ReactJS (Frontend)

**OAuth 2.0** là một giao thức xác thực cho phép người dùng đăng nhập vào ứng dụng bằng tài khoản của bên thứ ba (như Google) mà không cần chia sẻ mật khẩu. Google OAuth 2.0 giúp ứng dụng E-commerce cung cấp tính năng đăng nhập nhanh, an toàn, và lấy thông tin người dùng (email, tên). Tài liệu này hướng dẫn cách tích hợp Google OAuth 2.0 trong backend (.NET) và frontend (ReactJS).

---

## Yêu cầu

1. **Backend (.NET)**:
   - Cài đặt các gói NuGet:
     ```bash
     dotnet add package Microsoft.AspNetCore.Authentication.Google
     dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
     dotnet add package Microsoft.AspNetCore.Authentication.Cookies
     dotnet add package System.IdentityModel.Tokens.Jwt
     dotnet add package Dapper
     ```
2. **Frontend (ReactJS)**:
   - Cài đặt thư viện `@react-oauth/google`:
     ```bash
     npm install @react-oauth/google
     ```
3. **Google Cloud Console**:
   - Tạo một dự án để lấy `ClientId` và `ClientSecret`.

---

## Bước 1: Cấu hình Google Cloud Console

1. Truy cập [Google Cloud Console](https://console.cloud.google.com/).
2. Tạo dự án mới hoặc chọn dự án hiện có.
3. Vào **APIs & Services > Credentials**:
   - Tạo **OAuth 2.0 Client IDs** với loại **Web application**.
   - Thêm **Authorized redirect URIs** (ví dụ: `https://localhost:5001/auth/login-google-callback` cho backend).
   - Lưu `ClientId` và `ClientSecret`.
4. Cấu hình **OAuth consent screen**:
   - Chọn **External** và điền thông tin ứng dụng (như tên, logo).

---

## Bước 2: Cấu hình Backend (.NET)

### 2.1. Lưu thông tin Google OAuth trong `appsettings.json`

Cấu hình thông tin Google OAuth và JWT trong `appsettings.json`:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "0000000000xxx.apps.googleusercontent.com",
      "ClientSecret": "111111111111xxx",
      "ValidIssuer": "https://accounts.google.com",
      "ValidAudience": "0000000000xxx.apps.googleusercontent.com"
    },
    "JWT": {
      "issuer": "https://your-api.com",
      "audience": "https://your-api.com",
      "secret": "your-secure-jwt-secret-key-32-chars-long"
    }
  }
}
```

#### Giải thích cấu hình

| Tham số           | Ý nghĩa                                      | Ví dụ                                      |
|-------------------|----------------------------------------------|--------------------------------------------|
| `ClientId`        | ID ứng dụng từ Google Cloud Console.         | `0000000000xxx.apps.googleusercontent.com` |
| `ClientSecret`    | Khóa bí mật từ Google Cloud Console.         | `111111111111xxx`                         |
| `ValidIssuer`     | Nhà cung cấp token (Google).                 | `https://accounts.google.com`             |
| `ValidAudience`   | Đối tượng sử dụng token (ClientId).          | `0000000000xxx.apps.googleusercontent.com` |
| `JWT:issuer`      | Nhà phát hành token của bạn.                 | `https://your-api.com`                    |
| `JWT:audience`    | Đối tượng sử dụng token của bạn.             | `https://your-api.com`                    |
| `JWT:secret`      | Khóa bí mật để ký JWT.                       | `your-secure-jwt-secret-key-32-chars-long` |

**Lưu ý**: Trong môi trường production, lưu `ClientSecret` và `JWT:secret` trong biến môi trường hoặc Azure Key Vault để tăng bảo mật.

### 2.2. Đăng ký dịch vụ trong `Program.cs`

Cấu hình xác thực Google, JWT, và cookie trong `Program.cs`:

```csharp
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ xác thực
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "E_commerce.Cookie";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsProduction()
        ? CookieSecurePolicy.Always
        : CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
    options.LoginPath = "/auth/login";
    options.LogoutPath = "/auth/logout";
    options.AccessDeniedPath = "/auth/forbidden";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Authentication:JWT:issuer"],
        ValidAudience = builder.Configuration["Authentication:JWT:audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                builder.Configuration["Authentication:JWT:secret"] ??
                throw new InvalidOperationException("JWT Key is not found")
            )
        ),
        ClockSkew = TimeSpan.FromSeconds(30)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
})
.AddGoogle(options =>
{
    var clientId = builder.Configuration["Authentication:Google:ClientId"] ??
        throw new InvalidOperationException("Google ClientId is not found");
    var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ??
        throw new InvalidOperationException("Google ClientSecret is not found");

    options.ClientId = clientId;
    options.ClientSecret = clientSecret;
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});

// Đăng ký dịch vụ Google
builder.Services.AddScoped<IGoogleServiceAuthentication, GoogleServiceAuthentication>();
builder.Services.AddHttpClient<IGoogleServiceAuthentication, GoogleServiceAuthentication>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
```

#### Giải thích:
- **`AddAuthentication`**: Thiết lập các scheme mặc định cho xác thực (cookie) và challenge (Google).
- **`AddCookie`**: Lưu thông tin đăng nhập vào cookie an toàn (`HttpOnly = true`, `SecurePolicy`).
- **`AddJwtBearer`**: Xác thực token JWT do backend phát hành.
- **`AddGoogle`**: Tích hợp Google OAuth 2.0.

### 2.3. Định nghĩa giao diện `IGoogleServiceAuthentication`

Tạo tệp `IGoogleServiceAuthentication.cs`:

```csharp
namespace E_commerce.Infrastructure.Services
{
    public interface IGoogleServiceAuthentication
    {
        /// <summary>
        /// Xác thực Google ID token và lấy email, tên.
        /// </summary>
        Task<(string? Email, string? Name)> VerifyGoogleToken(GoogleVerificationDTO googleVerificationDTO);

        /// <summary>
        /// Kiểm tra email người dùng trong database, trả về thông tin nếu tồn tại.
        /// </summary>
        Task<_User> CheckVerifyAccountViaEmail(string email);

        /// <summary>
        /// Lấy hoặc tạo người dùng dựa trên email và tên.
        /// </summary>
        Task<_User> GetOrCreateUser(string email, string name);

        /// <summary>
        /// Đăng nhập bằng Google, trả về access token và refresh token.
        /// </summary>
        Task<(TokenDTO AccessToken, TokenDTO RefreshToken)> Login(ClaimsPrincipal? claimsPrincipal);
    }
}
```

### 2.4. Triển khai dịch vụ `GoogleServiceAuthentication`

Tạo tệp `GoogleServiceAuthentication.cs`:

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using E_commerce.Application.DTOs.Common;
using E_commerce.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Dapper;

namespace E_commerce.Infrastructure.Services
{
    public class GoogleServiceAuthentication : IGoogleServiceAuthentication
    {
        private readonly DatabaseConnectionFactory _connectionFactory;
        private readonly ILogger<GoogleServiceAuthentication> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private JsonWebKeySet _jsonWebKeySet;
        private readonly object _lock = new object();
        private DateTime _lastKeyFetch = DateTime.MinValue;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITokenService _tokenService;
        private readonly ITokenListManagementService _tokenListService;

        public GoogleServiceAuthentication(
            DatabaseConnectionFactory connectionFactory,
            ILogger<GoogleServiceAuthentication> logger,
            ICustomerRepository customerRepository,
            IConfiguration configuration,
            HttpClient httpClient,
            ITokenService tokenService,
            ITokenListManagementService tokenListService)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jsonWebKeySet = new JsonWebKeySet();
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _tokenListService = tokenListService ?? throw new ArgumentNullException(nameof(tokenListService));
        }

        public async Task<_User> CheckVerifyAccountViaEmail(string email)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<_User>(
                    UserQueries.FindUserByEmail,
                    new { email });
                return result ?? throw new InvalidOperationException($"User with email {email} not found.");
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Database error when retrieving user with email: {Email}", email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with email: {Email}", email);
                throw;
            }
        }

        public async Task<(string? Email, string? Name)> VerifyGoogleToken(GoogleVerificationDTO googleVerificationDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(googleVerificationDTO.IdToken))
                {
                    throw new ArgumentException("ID token is required.", nameof(googleVerificationDTO.IdToken));
                }

                // Cache khóa công khai
                JsonWebKeySet keysToReturn;
                lock (_lock)
                {
                    if (_lastKeyFetch.AddHours(1) >= DateTime.UtcNow)
                    {
                        keysToReturn = _jsonWebKeySet;
                    }
                    else
                    {
                        keysToReturn = null;
                    }
                }

                if (keysToReturn == null)
                {
                    var json = await _httpClient.GetStringAsync("https://www.googleapis.com/oauth2/v3/certs");
                    lock (_lock)
                    {
                        _jsonWebKeySet = JsonSerializer.Deserialize<JsonWebKeySet>(json) ??
                            throw new JsonException("Cannot deserialize Google public keys.");
                        _lastKeyFetch = DateTime.UtcNow;
                        keysToReturn = _jsonWebKeySet;
                    }
                }

                // Xác thực token
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _configuration["Authentication:Google:ValidIssuer"],
                    ValidAudience = _configuration["Authentication:Google:ClientId"],
                    IssuerSigningKeys = keysToReturn.Keys
                };

                var handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(googleVerificationDTO.IdToken, validationParameters, out var validatedToken);

                var email = principal.FindFirstValue(ClaimTypes.Email);
                var name = principal.FindFirstValue(ClaimTypes.Name);

                if (string.IsNullOrEmpty(email))
                {
                    throw new SecurityTokenException("Email not found in token.");
                }

                return (email, name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying Google token.");
                throw;
            }
        }

        public async Task<_User> GetOrCreateUser(string email, string name)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<_User>(
                    UserQueries.GetOrCreateUserByEmail,
                    new { email, user_name = name });

                return result ?? throw new InvalidOperationException($"Failed to get or create user with email {email}.");
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Database error when getting or creating user with email: {Email}", email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting or creating user with email: {Email}", email);
                throw;
            }
        }

        public async Task<(TokenDTO, TokenDTO)> Login(ClaimsPrincipal? claimsPrincipal)
        {
            try
            {
                if (claimsPrincipal == null)
                {
                    throw new InvalidOperationException("ClaimsPrincipal is null.");
                }

                var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
                var name = claimsPrincipal.FindFirstValue(ClaimTypes.Name);

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
                {
                    throw new InvalidOperationException("Email or name is missing from claims.");
                }

                var user = await GetOrCreateUser(email, name);

                var account = new AccountInforDTO
                {
                    user_id = user.user_id,
                    user_name = user.user_name,
                    email = user.email
                };

                var accessToken = await _tokenService.GenerateToken(account, 3);
                var refreshToken = await _tokenService.GenerateToken(account, 24 * 30);

                double accessTokenScore = CustomFormat.ConvertDateTimeToUnixTimestamp(accessToken.expiration);
                double refreshTokenScore = CustomFormat.ConvertDateTimeToUnixTimestamp(refreshToken.expiration);
                await _tokenListService.AddTokenToSortedSet(accessToken.token, accessTokenScore);
                await _tokenListService.AddTokenToSortedSet(refreshToken.token, refreshTokenScore);

                return (accessToken, refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google login failed.");
                throw;
            }
        }
    }
}
```

### 2.5. Tạo Controller xử lý đăng nhập Google

Tạo tệp `AuthenticationController.cs`:

```csharp
using E_commerce.Api.Model;
using E_commerce.Application.DTOs.Common;
using E_commerce.Core.Entities;
using E_commerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Controllers
{
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IGoogleServiceAuthentication _googleServiceAuthentication;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(
            IGoogleServiceAuthentication googleServiceAuthentication,
            IConfiguration configuration,
            ILogger<AuthenticationController> logger)
        {
            _googleServiceAuthentication = googleServiceAuthentication ?? throw new ArgumentNullException(nameof(googleServiceAuthentication));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private CookieOptions CreateSecureCookieOptions(DateTime expires)
        {
            return new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = _configuration.GetValue<bool>("Environment:IsProduction", false),
                SameSite = SameSiteMode.Lax,
                Expires = expires
            };
        }

        [HttpGet("login-google")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public IActionResult LoginGoogle([FromQuery] string returnUrl)
        {
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Authentication", new { returnUrl }, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("login-google-callback")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GoogleCallback([FromQuery] string returnUrl)
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Google authentication failed.");
                    return Unauthorized(new ApiResponse<object> { Success = false, Message = "Google authentication failed." });
                }

                var tokens = await _googleServiceAuthentication.Login(result.Principal);
                var accessToken = tokens.Item1;
                var refreshToken = tokens.Item2;

                Response.Cookies.Append("access_token", accessToken.token, CreateSecureCookieOptions(DateTime.UtcNow.AddHours(3)));
                Response.Cookies.Append("refresh_token", refreshToken.token, CreateSecureCookieOptions(DateTime.UtcNow.AddDays(30)));

                return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Google callback.");
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "An error occurred during Google login." });
            }
        }
    }
}
```

---

## Bước 3: Cấu hình Frontend (ReactJS)

### 3.1. Cài đặt thư viện

Cài đặt `@react-oauth/google` để xử lý đăng nhập Google:

```bash
npm install @react-oauth/google
```

### 3.2. Tạo Component đăng nhập Google

Tạo tệp `GoogleLoginButton.jsx`:

```jsx
import { GoogleLogin, GoogleOAuthProvider } from '@react-oauth/google';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const GoogleLoginButton = () => {
    const navigate = useNavigate();
    const clientId = '0000000000xxx.apps.googleusercontent.com'; // Lấy từ Google Cloud Console

    const handleSuccess = async (credentialResponse) => {
        try {
            // Gửi ID token đến backend
            const response = await axios.post('https://localhost:5001/auth/login-google', {
                idToken: credentialResponse.credential
            }, {
                withCredentials: true // Gửi cookie
            });

            // Lưu thông tin người dùng hoặc token (nếu cần)
            console.log('Login successful:', response.data);

            // Chuyển hướng sau khi đăng nhập thành công
            navigate('/dashboard');
        } catch (error) {
            console.error('Google login failed:', error);
        }
    };

    const handleError = () => {
        console.error('Google login error');
    };

    return (
        <GoogleOAuthProvider clientId={clientId}>
            <GoogleLogin
                onSuccess={handleSuccess}
                onError={handleError}
                useOneTap
            />
        </GoogleOAuthProvider>
    );
};

export default GoogleLoginButton;
```

### 3.3. Cấu hình Axios để gửi yêu cầu

Tạo tệp `api.js` để cấu hình Axios với cookie:

```javascript
import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:5001',
    withCredentials: true // Gửi cookie trong mọi yêu cầu
});

export default api;
```

### 3.4. Sử dụng trong ứng dụng

Thêm `GoogleLoginButton` vào trang đăng nhập (`LoginPage.jsx`):

```jsx
import GoogleLoginButton from './GoogleLoginButton';

const LoginPage = () => {
    return (
        <div>
            <h1>Login</h1>
            <GoogleLoginButton />
        </div>
    );
};

export default LoginPage;
```

---

## Bước 4: Kiểm tra tích hợp

1. **Backend**:
   - Chạy ứng dụng .NET (`dotnet run`).
   - Truy cập `https://localhost:5001/auth/login-google?returnUrl=/dashboard`.
   - Đăng nhập bằng Google, kiểm tra cookie (`access_token`, `refresh_token`) trong trình duyệt.

2. **Frontend**:
   - Chạy ứng dụng React (`npm start`).
   - Nhấn nút đăng nhập Google, kiểm tra console và cookie.

3. **Kiểm tra log**:
   - Xem log backend (`ILogger`) để xác nhận đăng nhập thành công hoặc lỗi.
   - Kiểm tra database để đảm bảo người dùng được tạo/lấy đúng.

---

## Xử lý lỗi thường gặp

| Lỗi                          | Nguyên nhân                              | Cách khắc phục                              |
|------------------------------|------------------------------------------|--------------------------------------------|
| `Invalid redirect URI`       | Redirect URI không khớp trong Google Cloud Console. | Cập nhật URI trong Google Cloud Console.   |
| `Token invalid`              | ID token sai hoặc hết hạn.               | Kiểm tra `IdToken` từ frontend.            |
| `Cookie not sent`            | `Secure = true` nhưng không dùng HTTPS.  | Dùng HTTPS hoặc đặt `Secure = false` trong dev. |
| `CORS error`                 | Backend không cho phép origin của frontend. | Thêm CORS policy trong `Program.cs`.       |

---

## Lưu ý

- **Bảo mật**:
  - Đặt `HttpOnly = true` và `Secure = true` cho cookie trong production.
  - Lưu `ClientSecret` và `JWT:secret` trong biến môi trường.
- **Tối ưu**:
  - Cache khóa công khai của Google trong Redis thay vì biến tĩnh.
  - Giới hạn quyền truy cập endpoint `/auth/login-google` bằng CORS.
- **Kiểm thử**:
  - Viết unit test cho `GoogleServiceAuthentication` bằng cách mock `HttpClient` và `ITokenService`.