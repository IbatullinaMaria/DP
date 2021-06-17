using System;
using NATS.Client;
using System.Text;
using Valuator;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RankCalculator
{
    public class RankCalculator
    {
        private readonly ISubscriber<MsgHandlerEventArgs> _subscr;
        private const string RankCalculatorQueue = "rank_calculator";
        private readonly IStorage _storage;
        private readonly IPublisher _pub;

        public RankCalculator(IStorage storage, ISubscriber<MsgHandlerEventArgs> sub, IPublisher pub)
        {
            _storage = storage;

            _subscr = sub;
            _pub = pub;
            _subscr.SubscribeAsyncWithQueue(Constants.RankCalculatorEventName, RankCalculatorQueue, (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                string text = _storage.GetValue(Constants.TextKeyPrefix + id);
                string idWithRankPrefix = Constants.RankKeyPrefix + id;
                
                var rank = GetRank(text);
                _storage.StoreValue(idWithRankPrefix, rank.ToString());

                var jsonUtf8Bytes = SerializeRankInfo(idWithRankPrefix, rank.ToString());
                _pub.Publish(Constants.RankCalculatedEventName, jsonUtf8Bytes);         
            });
        }
        private static byte[] SerializeRankInfo(string id, string rankValue)
        {
            RankInfo info = new RankInfo()
            {
                rank = rankValue,
                contextId = id
            };
            return JsonSerializer.SerializeToUtf8Bytes(info);
        }
        public void Start()
        {
            Console.WriteLine("RankCalculator service started");

            _subscr.Start();

            Console.WriteLine("Press Enter to exit RankCalculator");
            Console.ReadLine();

            _subscr.Unsubscribe();
        }

        private double GetRank(string text)
        {
            int nonalphaCounter = 0;
            foreach (var ch in text)
            {
                if (!Char.IsLetter(ch))
                {
                    nonalphaCounter++;
                }
            }
            var rank = Convert.ToDouble(nonalphaCounter) / Convert.ToDouble(text.Length);

            string logText = $"Text: {text.Substring(0, Math.Min(10, text.Length))} Length: {text.Length} Non alpha count: {nonalphaCounter} Rank {rank}";
            Console.WriteLine(logText);

            return rank;
        }
    }
}