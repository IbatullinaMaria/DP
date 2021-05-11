using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Valuator;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;
        
        private readonly IPublisher _publisher;
        private readonly string _textsSetKey = "Set-text-";

        public IndexModel(ILogger<IndexModel> logger, IStorage storage, IPublisher publisher)
        {
            _logger = logger;
            _storage = storage;
            _publisher = publisher;
        }

        public void OnGet()
        {

        }
        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString(); 

            string similarityKey = Constants.SimilarityKeyPrefix + id;
            string similarity = GetSimilarity(text).ToString();
            _storage.StoreValue(similarityKey, similarity);

            string textKey = Constants.TextKeyPrefix + id;
            _storage.StoreValue(textKey, text);
            _storage.StoreToSet(_textsSetKey, text);
            byte[] data = Encoding.UTF8.GetBytes(id);
            _publisher.Publish(Constants.RankCalculatorEventName, data);

            return Redirect($"summary?id={id}");
        }

        

        private double GetSimilarity(string text)
        {
            return _storage.ExistingText(_textsSetKey, text) ? 1 : 0;
        }
    }
}
