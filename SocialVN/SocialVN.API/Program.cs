﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SocialVN.API.Data;
using SocialVN.API.Mappings;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using SocialVN.API.Repositories;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;




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
    options.UseInlineDefinitionsForEnums();
    options.EnableAnnotations();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token theo định dạng: \"{token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });


    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" 
                },
                Scheme = "bearer",
                Name = "Authorization",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

});
//builder.Services.AddDbContext<SocialVNDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SocialVNConnectionString")));
builder.Services.AddDbContext<SocialVNDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("SocialVNConnectionStrings"),
    new MySqlServerVersion(new Version(8, 0, 41))));

//builder.Services.AddDbContext<SocialVNAuthDbContext>(options =>
//    options.UseMySql(builder.Configuration.GetConnectionString("SocialVNAuthConnectionStrings"),
//    new MySqlServerVersion(new Version(8, 0, 41))));

//Đăng ký AutoMapper và thêm cấu hình ánh xạ từ AutoMapperProfiles
builder.Services.AddAutoMapper(typeof(AutomapperProfiles));
builder.Services.AddScoped<IImageRepository, SQLImageRepository>();

builder.Services.AddScoped<ITokenRepository, TokenRepository>();
//builder.Services.AddScoped<IUserRepository, SQLUserRepository>(); 
builder.Services.AddScoped<IPostRepository, SQLPostRepository>();
builder.Services.AddScoped<ICommentRepository, SQLCommentRepository>();
builder.Services.AddScoped<IFriendshipRepository, SQLFriendshipRepository>();
builder.Services.AddScoped<ILikeRepository, SQLLikeRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
// Đăng ký Identity Core với IdentityUser và thêm các dịch vụ liên quan
builder.Services.AddIdentityCore<ApplicationUser>()
    // Thêm dịch vụ quản lý vai trò cho IdentityRole
    .AddRoles<IdentityRole>()
    // Thêm dịch vụ Token Provider với tên "SocialVN"
    .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("SocialVN")
    // Đăng ký EntityFramework Store cho SocialVNAuthDbContext
    .AddEntityFrameworkStores<SocialVNDbContext>()
    // Thêm các Token Provider mặc định
    .AddDefaultTokenProviders();


// Cấu hình các tùy chọn cho Identity, đặt các yêu cầu cho mật khẩu
builder.Services.Configure<IdentityOptions>(
    options =>
    {
        options.Password.RequireDigit = false; 
        options.Password.RequireLowercase = false; 
        options.Password.RequireNonAlphanumeric = false; 
        options.Password.RequireUppercase = false; 
        options.Password.RequiredLength = 6; 
        options.Password.RequiredUniqueChars = 1; 
    }
);


// Cấu hình Authentication với JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            NameClaimType = ClaimTypes.NameIdentifier,
            IssuerSigningKey = new SymmetricSecurityKey(
           Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))

        };
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse(); // chặn response mặc định
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new ApiResponse<string>(401, "Bạn không có quyền truy cập.", null));
                return context.Response.WriteAsync(result);
            }
        };
    }
   
);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.WriteIndented = true;
    });


builder.Services.AddHttpContextAccessor();

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
