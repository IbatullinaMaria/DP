using System;
using Valuator;
namespace EventsLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            NatsSubscriber natsSubscriber = new NatsSubscriber();
            EventsLogger eventsLogger = new EventsLogger(natsSubscriber);
            eventsLogger.Start();      
        }
    }
}
