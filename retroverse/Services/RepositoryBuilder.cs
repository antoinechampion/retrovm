using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RetroVm.Server.Services
{
    public class RepositoryBuilder
    {
        private readonly ILogger _logger;

        private readonly List<Aggregator> _aggregators = new List<Aggregator>();

        public RepositoryBuilder(ILogger<RepositoryBuilder> logger)
        {
            _logger = logger;
        }

        public async Task BuildUsingAggregators(string outFolderPath, bool useGamulator = true)
        {
            _aggregators.Clear();
            if (useGamulator)
            {
                var outPath = Path.Combine(outFolderPath, "gamulator.yml");
                _aggregators.Add(new GamulatorAggregator(outPath));
            }

            _logger.LogInformation("Starting aggregation...");
            foreach (var aggregator in _aggregators)
            {
               await aggregator.AggregateAllGames(_logger);
            }
        }
    }
}
