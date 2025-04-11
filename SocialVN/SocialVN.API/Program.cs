using Microsoft.EntityFrameworkCore;
using SocialVN.API.Data;
using SocialVN.API.Mappings;
using SocialVN.API.Repositories;
using System.Reflection;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations(); // ✅ Để dùng SwaggerOperation, SwaggerResponse
});
//builder.Services.AddDbContext<SocialVNDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SocialVNConnectionString")));
builder.Services.AddDbContext<SocialVNDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("SocialVNConnectionStrings"),
    new MySqlServerVersion(new Version(8, 0, 41))));
//Đăng ký AutoMapper và thêm cấu hình ánh xạ từ AutoMapperProfiles
builder.Services.AddAutoMapper(typeof(AutomapperProfiles));

//Tiêm các kho lưu trữ vào bộ điều khiển
builder.Services.AddScoped<IUserRepository, SQLUserRepository>(); // Đăng ký dịch vụ
builder.Services.AddScoped<IPostRepository, SQLPostRepository>();
builder.Services.AddScoped<ICommentRepository, SQLCommentRepository>();
builder.Services.AddScoped<IFriendshipRepository, SQLFriendshipRepository>();
builder.Services.AddScoped<ILikeRepository, SQLLikeRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
