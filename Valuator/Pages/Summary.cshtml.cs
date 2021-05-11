using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        private readonly IStorage _storage;

        public SummaryModel(ILogger<SummaryModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public double Similarity { get; set; }
        public double Rank { get; set; }

        public void OnGet(string id)
        {
            _logger.LogDebug(id);

            string rankKey = Constants.RankKeyPrefix + id;
            Rank = Convert.ToDouble(_storage.LoadValue(rankKey));

            string similarityKey = Constants.SimilarityKeyPrefix + id;
            Similarity = Convert.ToDouble(_storage.LoadValue(similarityKey));
        }
    }
}
