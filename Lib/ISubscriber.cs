using System;

namespace Valuator

{
    public interface ISubscriber<Args>
    {
        void SubscribeAsyncWithQueue(string subject, string queue, EventHandler<Args> handler);

        void SubscribeAsync(string subject, EventHandler<Args> handler);

        void Start();
        
        void Unsubscribe();

    }
}