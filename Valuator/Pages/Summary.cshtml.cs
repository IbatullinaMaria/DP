using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        private readonly IStorage _storage;
        private const int RetryCount = 1000;
        private readonly TimeSpan Delay = TimeSpan.FromSeconds(0.01);
        public SummaryModel(ILogger<SummaryModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }

        public void OnGet(string id)
        {
            _logger.LogDebug(id);

            Console.WriteLine("LOOKUP: {0}, {1}", id, _storage.GetShardId(id));
            var rank = GetRankValue(id);
            if (String.IsNullOrEmpty(rank))
            {
                _logger.LogDebug("Cannot get rank value");
            }
            else
            {
                Rank = Math.Round(Convert.ToDouble(_storage.GetValue(id, Constants.RankKeyPrefix + id)), 2);
            }
            Similarity = Math.Round(Convert.ToDouble(_storage.GetValue(id, Constants.SimilarityKeyPrefix + id)), 2);
        }

        private string GetRankValue(string id)
        {
            int currentRetry = 0;
            string rank;
            while(currentRetry < RetryCount)
            {
                rank = _storage.GetValue(id, Constants.RankKeyPrefix + id);
                if (!String.IsNullOrEmpty(rank))
                { 
                    return rank;
                }
                else 
                {
                    Thread.Sleep(Delay);
                    currentRetry++;
                }
            }
            return String.Empty;
        }
    }
}
