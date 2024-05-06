using DemoCleanArchitecture.Domain.Interfaces;
using DemoCleanArchitecture.Infrastructure.Data;
using DemoCleanArchitecture.Infrastructure.Extensions;
using DemoCleanArchitecture.Infrastructure.MessageQueue;
using DemoCleanArchitecture.Infrastructure.Middleware;
using DemoCleanArchitecture.Worker;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAppServices();
builder.Services.AddDbContext<DataContext>(
    (context) =>
    {
        context.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")
        ); ;
    }
);

// Đăng ký dịch vụ phân quyền
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
});
//config jwt trong swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Thêm thông tin xác thực
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {token}'",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    var securityRequirement = new OpenApiSecurityRequirement
    {
        { securityScheme, new[] { "Bearer" } }
    };
    c.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddHttpContextAccessor();

//builder.Services.AddSingleton<IMessageQueue, RabbitMQMessageQueue>(provider =>
//{
//    var configuration = provider.GetRequiredService<IConfiguration>();
//    var hostName = configuration["RabbitMQ:HostName"];
//    var queueName = configuration["RabbitMQ:QueueName"];
//    var loggerFactory = provider.GetRequiredService<ILoggerFactory>(); // Đây là phần sửa, thêm <ILoggerFactory> để cung cấp kiểu dịch vụ cụ thể.
//    var logger = loggerFactory.CreateLogger<RabbitMQMessageQueue>();
//    // Khởi tạo và trả về một RabbitMQMessageQueue với các thông số đã cấu hình
//    return new RabbitMQMessageQueue(hostName, queueName, logger);
//});

builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();


builder.Services.AddSingleton<SmtpClient>(provider =>
{
    // Truy cập Configuration để lấy các thông tin cấu hình của SmtpClient từ appsettings hoặc các nguồn khác
    var configuration = provider.GetRequiredService<IConfiguration>();
    var smtpClient = new SmtpClient();
    //smtpClient.Host = configuration["SmtpConfig:HostName"];
    smtpClient.Port = int.Parse(configuration["SmtpConfig:Port"]);
    //smtpClient.EnableSsl = bool.Parse(configuration["SmtpConfig:EnableSsl"]);
    smtpClient.Credentials = new System.Net.NetworkCredential(
        configuration["SmtpConfig:Username"],
        configuration["SmtpConfig:Password"]
    );
    return smtpClient;
});
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
