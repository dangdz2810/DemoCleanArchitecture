using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCleanArchitecture.Infrastructure.MessageQueue
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItemAsync(Func<IServiceScopeFactory, CancellationToken, Task> workItem);
        Task<Func<IServiceScopeFactory, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
