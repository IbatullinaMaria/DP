using System;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly string _textsSetKey = "Set-text-";

        public IndexModel(ILogger<IndexModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet()
        {

        }
        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            string rankKey = "Rank-" + id;
            string rank = CalculateRank(text).ToString();
            _storage.StoreValue(rankKey, rank);

            string similarityKey = "Similarity-" + id;
            string similarity = GetSimilarity(text).ToString();
            _storage.StoreValue(similarityKey, similarity);

            string textKey = "Text-" + id;
            _storage.StoreValue(textKey, text);
            _storage.StoreToSet(_textsSetKey, text);

            return Redirect($"summary?id={id}");
        }

        private double CalculateRank(string text)
        {
            double length = text.Length, notCharsCount = 0;
            for (int i = 0; i != length; ++i)
            {
                if (!Char.IsLetter(text[i]))
                    ++notCharsCount;
            }
            return Math.Round(notCharsCount / length, 2);
        }

        private double GetSimilarity(string text)
        {
            return _storage.ExistingText(_textsSetKey, text) ? 1 : 0;
        }
    }
}
