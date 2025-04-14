using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SocialVN.API.Data;
using SocialVN.API.Mappings;
using SocialVN.API.Models.Domain;
using SocialVN.API.Repositories;
using System.Reflection;
using System.Text;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SocialVN API",
        Version = "v1",
        Description = "API cho mạng xã hội SocialVN"
    });
    options.EnableAnnotations();
    options.AddSecurityDefinition(
    JwtBearerDefaults.AuthenticationScheme,
    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

});
//builder.Services.AddDbContext<SocialVNDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SocialVNConnectionString")));
builder.Services.AddDbContext<SocialVNDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("SocialVNConnectionStrings"),
    new MySqlServerVersion(new Version(8, 0, 41))));

builder.Services.AddDbContext<SocialVNAuthDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("SocialVNAuthConnectionStrings"),
    new MySqlServerVersion(new Version(8, 0, 41))));

//Đăng ký AutoMapper và thêm cấu hình ánh xạ từ AutoMapperProfiles
builder.Services.AddAutoMapper(typeof(AutomapperProfiles));

//Tiêm các kho lưu trữ vào bộ điều khiển
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IUserRepository, SQLUserRepository>(); // Đăng ký dịch vụ
builder.Services.AddScoped<IPostRepository, SQLPostRepository>();
builder.Services.AddScoped<ICommentRepository, SQLCommentRepository>();
builder.Services.AddScoped<IFriendshipRepository, SQLFriendshipRepository>();
builder.Services.AddScoped<ILikeRepository, SQLLikeRepository>();

// Đăng ký Identity Core với IdentityUser và thêm các dịch vụ liên quan
builder.Services.AddIdentityCore<ApplicationUser>()
    // Thêm dịch vụ quản lý vai trò cho IdentityRole
    .AddRoles<IdentityRole>()
    // Thêm dịch vụ Token Provider với tên "SocialVN"
    .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("SocialVN")
    // Đăng ký EntityFramework Store cho SocialVNAuthDbContext
    .AddEntityFrameworkStores<SocialVNAuthDbContext>()
    // Thêm các Token Provider mặc định
    .AddDefaultTokenProviders();


// Cấu hình các tùy chọn cho Identity, đặt các yêu cầu cho mật khẩu
builder.Services.Configure<IdentityOptions>(
    options =>
    {
        options.Password.RequireDigit = false; // Không yêu cầu mật khẩu phải có chữ số
        options.Password.RequireLowercase = false; // Không yêu cầu mật khẩu phải có chữ cái viết thường
        options.Password.RequireNonAlphanumeric = false; // Không yêu cầu mật khẩu phải có ký tự không phải là chữ hoặc số
        options.Password.RequireUppercase = false; // Không yêu cầu mật khẩu phải có chữ cái viết hoa
        options.Password.RequiredLength = 6; // Mật khẩu phải dài ít nhất 6 ký tự
        options.Password.RequiredUniqueChars = 1; // Mật khẩu phải có ít nhất 1 ký tự khác biệt
    }
);


// Cấu hình Authentication với JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
    }
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Kích hoạt middleware xác thực để xử lý yêu cầu xác thực người dùng
app.UseAuthentication();

// Kích hoạt middleware ủy quyền để kiểm tra quyền truy cập của người dùng đã xác thực
app.UseAuthorization();

app.MapControllers();

app.Run();
