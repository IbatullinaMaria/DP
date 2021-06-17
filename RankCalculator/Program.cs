using System;
using NATS.Client;
using System.Text;
using Valuator;
namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            RedisStorage storage = new RedisStorage();
            NatsSubscriber natsSubscriber = new NatsSubscriber();
            NatsPublisher natsPublisher = new NatsPublisher();
            RankCalculator RankCalculator = new RankCalculator(storage, natsSubscriber, natsPublisher);
            RankCalculator.Start();
        }
    }
}
