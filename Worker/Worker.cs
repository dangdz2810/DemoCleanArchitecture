//using DemoCleanArchitecture.Domain.Interfaces;
using DemoCleanArchitecture.Infrastructure.MessageQueue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DemoCleanArchitecture.Worker
{
    public class Worker : BackgroundService, IDisposable
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public Worker(ILogger<Worker> logger, IBackgroundTaskQueue taskQueue, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _taskQueue = taskQueue;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //_messageQueue.Subscribe(HandleMessage);

                //// Dùng Task.Delay() để giữ cho service hoạt động mãi mãi
                //// Nếu bạn có điều kiện dừng khác, bạn có thể sử dụng stoppingToken.WaitHandle.WaitOne()
                //await Task.Delay(1000, stoppingToken);
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(_serviceScopeFactory,stoppingToken);

                }
                catch (Exception ex)
                {
                    // Xử lý lỗi, có thể log lại hoặc gửi email thông báo lỗi
                    throw new Exception(ex.Message);
                }
            }
        }
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }


        //public override void Dispose()
        //{
        //    // Đảm bảo phương thức Dispose được gọi khi service dừng
        //    base.Dispose();
        //}
    }
}
