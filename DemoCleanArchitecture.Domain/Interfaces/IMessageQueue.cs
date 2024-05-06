using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCleanArchitecture.Domain.Interfaces
{
    public interface IMessageQueue
    {
        void Publish(string message);
        void Subscribe(Action<string> callback);
    }
}
