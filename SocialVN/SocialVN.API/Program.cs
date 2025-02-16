using Microsoft.EntityFrameworkCore;
using SocialVN.API.Data;
using SocialVN.API.Mappings;
using SocialVN.API.Repositories;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SocialVNDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SocialVNConnectionString")));
// Đăng ký AutoMapper và thêm cấu hình ánh xạ từ AutoMapperProfiles
builder.Services.AddAutoMapper(typeof(AutomapperProfiles));
builder.Services.AddScoped<IUserRepository, SQLUserRepository>(); // Đăng ký dịch vụ

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
