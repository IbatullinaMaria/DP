using NATS.Client;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuator;
using Microsoft.Extensions.Logging;

namespace RankCalculator
{
    class Program
    {
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