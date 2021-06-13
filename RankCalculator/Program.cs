using NATS.Client;
using Library;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuator;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace RankCalculator
{
    class Program
    {
        private static IDatabase _db;
        private static IPublisher _messageBroker;

        private static double CalcRank(string text) {
            double rank = 0;
            if (text != null) {
                int notLetterCharsCount = text.Where(ch => !char.IsLetter(ch)).Count();
                rank = notLetterCharsCount / (double) text.Length;
            }
            return rank;
        }

        private static void StoreRank(string id, string rank)
        {
            _db.StringSet(Constants.RankKeyPrefix + id, rank);
        }

        private static void PublishEventRankCalculated(string id, string rank)
        {
            EventContainer eventData = new EventContainer { Name = "RankCalculated", Id = id, Value = rank };
            _messageBroker.Send("Events", JsonSerializer.Serialize(eventData));
        }
        static void Main(string[] args)
        {
            Console.WriteLine("RankCalculator started");

            ConnectionFactory cf = new ConnectionFactory();
            using IConnection c = cf.CreateConnection();

            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = connectionMultiplexer.GetDatabase();

            var s = c.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                string textKey = Constants.TextKeyPrefix + id;
                string text = db.StringGet(textKey);
                double rank = 0;
                double length = text.Length, notCharsCount = 0;
                for (int i = 0; i != length; ++i)
                {
                    if (!Char.IsLetter(text[i]))
                        ++notCharsCount;
                }
                rank = Math.Round(notCharsCount / length, 2);
                //Console.WriteLine(rank);
                string rankKey = Constants.RankKeyPrefix + id;
                db.StringSet(rankKey, rank.ToString("0.##"));
            });

            s.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            s.Unsubscribe();

            c.Drain();
            c.Close();
        }
    }
}
/*
using System;
using NATS.Client;
using System.Text;
using Valuator;
namespace RankCalculatorService
{
    class Program
    {
        static void Main(string[] args)
        {
            RedisStorage storage = new RedisStorage();
            NatsSubscriber natsSubscriber = new NatsSubscriber();
            NatsPublisher natsPublisher = new NatsPublisher();
            RankCalculatorService rankCalculatorService = new RankCalculatorService(storage, natsSubscriber, natsPublisher);
            rankCalculatorService.Start();
        }
    }
}*/