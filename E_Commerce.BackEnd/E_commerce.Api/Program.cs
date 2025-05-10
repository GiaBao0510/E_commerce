using E_commerce.Infrastructure;
using E_commerce.Api.Middleware;
using E_commerce.Infrastructure.Data;
using log4net;
using log4net.Config;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using E_commerce.Api.Authentication;
using Microsoft.AspNetCore.Http.Features;
using E_commerce.Api.Hubs;


var builder = WebApplication.CreateBuilder(args);

//Cấu hình kerstrel để lắng nghe trên nhiều cổng
builder.WebHost.ConfigureKestrel(options => {
    //Http Endpoint
    var httpPort = builder.Configuration.GetValue<int>("Ports:Http", 5126);
    var httpsPort = builder.Configuration.GetValue<int>("Ports:Https", 5127);

    //HTTP/1.1 và HTTP/2
    options.ListenLocalhost(httpPort, listenOption =>{
        listenOption.Protocols = 
            Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});

//Configura Log4net.
XmlConfigurator.Configure(new FileInfo("log4net.config"));

//injecting services => Defined under CleanArch.Infrastructure.Project
builder.Services.RegisterServices(builder.Configuration);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();   //Hỗ trọ JsonPatch

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region ====[CORS]====
//Configure CORS
builder.Services.AddCors(options => {
    var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
        new []{
            "http://localhost:3000",        //React (FrontEnd)
            "https://localhost:3000" ,
            "http://localhost:4200",        //Angular (FrontEnd)
            "http://localhost:8080",        //Vue (FrontEnd)
            "http://localhost:3001",        //Next.js (FrontEnd)
            "http://localhost:5129"         //Swagger
        };
    options.AddPolicy("AllowedOrigin", policy => {
        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
#endregion

#region  ====[Swagger]====
//Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(options =>{
    options.SwaggerDoc("v1", new OpenApiInfo {Title = "API", Version = "v1"});
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement( new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

#region  ====[Cache]====
//Configure Cache
builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(options => {
    var redisHost= builder.Configuration["Database:Redis:Host"];
    var redisPassword = builder.Configuration["Database:Redis:Password"];

    options.Configuration = $"{redisHost}, password={redisPassword}";
    options.InstanceName = "SampleInstance";
}); 
#endregion

#region =====[SignalR]
builder.Services.AddSignalR();
#endregion

#region =====[API - Version]====
//Cấu hình service API - version
builder.Services.AddApiVersioning(options =>{
    options.DefaultApiVersion = new ApiVersion(1,0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options => {
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
#endregion

#region =====[API - Authentication]====
// Configure Authentication: JWT & Cookie && Google
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme =       //Quy định cơ chế được sử dụng để xác thực token khi 1 req đến API 
    options.DefaultChallengeScheme =          // Quy định cơ chế sẽ được sử dụng người dùng chưa đăng nhập cần chuyển đến trang đăng nhập
    options.DefaultForbidScheme =             // Quy định cơ chế sẽ được sử dụng khi người dùng không được phép truy cập vào tài nguyên
    options.DefaultScheme =                   // Cơ chế mặc định khi không có cơ chế nào được chỉ định   
    options.DefaultSignInScheme =             // Cơ chế sẽ được sử dụng khi người dùng đăng nhập
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters{
        ValidateIssuer = true,              // Kiểm tra xem ai là người phát hành token( thường là server)
        ValidateAudience = true,            // Kiểm tra xem token này dành cho đối tượng nào (thường là FE)
        ValidateIssuerSigningKey = true,    // Kiểm tra xem chữ ký của token để đảm bảo tính toàn vẹn
        ValidateLifetime = true,            // Kiểm tra xem token đã hết hạn chưa
        ValidIssuer = builder.Configuration["Authentication:JWT:issuer"],       // Issuer: Người phát hành token
        ValidAudience = builder.Configuration["Authentication:JWT:audience"],   // Audience: Đối tượng sử dụng token
        IssuerSigningKey = new SymmetricSecurityKey(                            // Khóa bí mật dùng để ký và xác minh token
            System.Text.Encoding.UTF8.GetBytes(
                builder.Configuration["Authentication:JWT:secret"] ??
                    throw new InvalidOperationException("JWT Key is not found")
            )
        ),

        //Xử lý sự kiện khi token không hợp lệ
        ClockSkew = TimeSpan.FromSeconds(30), // Thời gian chênh lệch giữa server và client
    };

    //Thêm xử lý sự kiện cho JWT
    options.Events = new JwtBearerEvents{
        OnAuthenticationFailed = context => {
            if(context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                context.Response.Headers.Append("Token-Expired", "true"); //Thêm header vào response nếu token đã hết hạn
            
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            //Hỗ trợ nhận token từ query string cho webSocket hoặc server-sent events
            var accessToken = context.Request.Query["access_token"];
            if(!string.IsNullOrEmpty(accessToken))
                context.Token = accessToken; //Gán token vào context để xác thực
            
            return Task.CompletedTask;
        }
    };
})
.AddCookie(options => {
        options.Cookie.Name = "E_commerce.Cookie";                  //Tên của cookie để phân biệt với các cookie khác
        options.Cookie.HttpOnly = true;                             // True: nghĩa là JS không thể truy cập vào cookie (bảo mật hơn)
        options.Cookie.SecurePolicy = builder.Environment.IsProduction()
            ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;    // Always: Nghĩa là cookie chỉ được gửi qua HTTPS (Bảo mật hơn)
        options.Cookie.SameSite = SameSiteMode.Lax;                 // Thuộc tính SameSite giúp ngăn chặn CSRF (Cross-Site Request Forgery)
        options.ExpireTimeSpan = TimeSpan.FromHours(24);            // Thời gian sống của cookie
        options.SlidingExpiration = true;                           // True: nghĩa là mỗi lần người dùng truy cập, thời hạn của cookie sẽ được gia hạn
        
        options.LoginPath = "/auth/login";   
        options.LogoutPath = "/auth/logout";                                   //Trang sẽ chuyển huong khi người dùng chưa đăng nhập
        options.AccessDeniedPath = "/auth/forbidden";                            //Trang sẽ chuyển hướng khi người dùng không có quyền truy cập vào tài nguyên
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options => {
    options.ClientId = builder.Configuration.GetSection("Authentication:Google:ClientId").Value;
    options.ClientSecret = builder.Configuration.GetSection("Authentication:Google:ClientSecret").Value;

    //Thêm scope để lấy thông tin người dùng
    options.Scope.Add("email");
    options.Scope.Add("profile");
    options.Scope.Add("openid");

    //Lưu Token từ Google để sử dụng sau này
    options.SaveTokens = true;

    //Callbacks URL cho google OAuth
    options.CallbackPath = "/auth/google-login-callback"; //URL sẽ được gọi lại sau khi người dùng đăng nhập thành công
});
#endregion

//Add health check
builder.Services.AddHealthChecks();


#region  ====[Middler ware]====
var app = builder.Build();

// Khởi động connection pool khi ứng dụng bắt đầu
{
    var connectionFactory = app.Services.GetRequiredService<DatabaseConnectionFactory>();
    connectionFactory.WarmupConnectionPool(); 
}

// Middleware pipeline order
// 1. Exception Handling & 
app.UseCustomExceptionHandler(); 

//2. Development Enviroment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "E_commerce v1"));
}
else{
    app.UseHsts();
}

//3. HttpsRedirection
app.UseHttpsRedirection();

//4. Staticfiles
app.UseStaticFiles();

//5. Routing
app.UseRouting();

//6. CORS
app.UseCors("AllowedOrigin");

//7. Authentication & Authorization
app.UseAuthentication();
app.UseTokenWhitelistValidation(); //Xác thực token có trong whitelist hay không 
app.UseAuthorization();

//8. Custome Middleware. Here


//9. Endpoints
app.MapHub<ChatHub>("/chat-hub"); //Map SignalR hub
app.MapControllers(); //Map all controllers
app.MapHealthChecks("/health"); //Map health check endpoint

app.Run();
#endregion