using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;
        private readonly IPublisher _publisher;
        public IndexModel(ILogger<IndexModel> logger, IStorage storage, IPublisher publisher)
        {
            _storage = storage;
            _logger = logger;
            _publisher = publisher;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return Redirect($"summary");
            }

            string id = Guid.NewGuid().ToString();
            
            ProcessSimilarity(id, text);

            StoreText(id, text);

            DelegateRankProcessing(id);

            return Redirect($"summary?id={id}");     
        }
        private void StoreText(string id, string text)
        {
            string textKey = Constants.TextKeyPrefix + id;
            _storage.StoreValue(textKey, text);
            _storage.StoreTextToSet(text);
        }
        private void DelegateRankProcessing(string id)
        {
            string rankKey = Constants.RankKeyPrefix + id;
            byte[] data = Encoding.UTF8.GetBytes(id);
            _publisher.Publish(Constants.RankCalculatorEventName, data);
        }
        private void ProcessSimilarity(string id, string text)
        {
            string similarityKey = Constants.SimilarityKeyPrefix + id;
            var similarity = GetSimilarity(text);
            _storage.StoreValue(similarityKey, similarity.ToString());
            var jsonUtf8Bytes = SerializeSimilarityInfo(similarityKey, similarity.ToString());
            _publisher.Publish(Constants.SimilarityCalculatedEventName, jsonUtf8Bytes);
        }

        private static byte[] SerializeSimilarityInfo(string id, string simValue)
        {
            SimilarityInfo info = new SimilarityInfo()
            {
                similarity = simValue,
                contextId = id
            };
            return JsonSerializer.SerializeToUtf8Bytes(info);
        }
        private double GetSimilarity(string text)
        {
            if (_storage.FindText(text))
            {
                return 1d;
            }
            return 0d;
        }
    }
}
