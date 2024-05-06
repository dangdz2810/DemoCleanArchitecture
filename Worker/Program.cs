using DemoCleanArchitecture.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DemoCleanArchitecture.Infrastructure.MessageQueue;
using Microsoft.Extensions.Logging;

namespace DemoCleanArchitecture.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddSingleton<IMessageQueue, RabbitMQMessageQueue>(provider =>
                    //{
                    //    // Truy cập Configuration để lấy các thông tin cần thiết, ví dụ như hostName và queueName
                    //    var configuration = provider.GetRequiredService<IConfiguration>();

                    //    // Lấy các thông số cấu hình từ appsettings hoặc các nguồn khác
                    //    var hostName = configuration["RabbitMQ:HostName"];
                    //    var queueName = configuration["RabbitMQ:QueueName"];
                    //    // Khởi tạo logger cho RabbitMQMessageQueue
                    //    // Khởi tạo logger cho RabbitMQMessageQueue
                    //    var loggerFactory = provider.GetRequiredService<ILoggerFactory>(); // Đây là phần sửa, thêm <ILoggerFactory> để cung cấp kiểu dịch vụ cụ thể.
                    //    var logger = loggerFactory.CreateLogger<RabbitMQMessageQueue>();
                    //    // Khởi tạo và trả về một RabbitMQMessageQueue với các thông số đã cấu hình
                    //    return new RabbitMQMessageQueue(hostName, queueName, logger); // Đảm bảo rằng lớp RabbitMQMessageQueue của bạn có thể nhận logger như một tham số.
                    //});
                    services.AddHostedService<Worker>();
                });
    }
}
