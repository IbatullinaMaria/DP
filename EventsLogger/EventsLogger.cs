using System;
using NATS.Client;
using System.Text;
using Valuator;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventsLogger
{
    public class EventsLogger
    {
        private readonly ISubscriber<MsgHandlerEventArgs> _subscr;
   
        public EventsLogger(ISubscriber<MsgHandlerEventArgs> sub)
        {
            _subscr = sub;

            _subscr.SubscribeAsync(Constants.SimilarityCalculatedEventName, _similarityCalculatedHandler);
            _subscr.SubscribeAsync(Constants.RankCalculatedEventName, _rankCalculatedHandler);
        }

        private EventHandler<MsgHandlerEventArgs> _similarityCalculatedHandler = (sender, args) =>
        {
            Console.WriteLine($"Event: {args.Message.Subject}");
            SimilarityInfo info = DeserializeSimilarityInfo(args.Message.Data);

            Console.WriteLine($"Context id: {info.contextId}");
            Console.WriteLine($"Similarity value: {info.similarity}");
            Console.WriteLine();
        };
        private EventHandler<MsgHandlerEventArgs> _rankCalculatedHandler = (sender, args) =>
        {
            Console.WriteLine($"Event: {args.Message.Subject}");
            RankInfo info = DeserializeRankInfo(args.Message.Data);

            Console.WriteLine($"Context id: {info.contextId}");
            Console.WriteLine($"Rank value: {info.rank}");
            Console.WriteLine();
        };

        private static SimilarityInfo DeserializeSimilarityInfo(byte[] jsonUtf8Bytes)
        {
            var readOnlySpan = new ReadOnlySpan<byte>(jsonUtf8Bytes);
            return JsonSerializer.Deserialize<SimilarityInfo>(readOnlySpan);
        }
        private static RankInfo DeserializeRankInfo(byte[] jsonUtf8Bytes)
        {
            var readOnlySpan = new ReadOnlySpan<byte>(jsonUtf8Bytes);
            return JsonSerializer.Deserialize<RankInfo>(readOnlySpan);
        }
        public void Start()
        {
            Console.WriteLine("Logger started");
            _subscr.Start();

            Console.WriteLine("Press Enter to exit Logger");
            Console.WriteLine();
            Console.ReadLine();

            _subscr.Unsubscribe();
        }
    }
}