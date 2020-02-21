using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RetroVm.Server.Services
{
    public class RepositoryBuilder
    {
        private readonly ILogger _logger;

        private readonly List<Aggregator> _aggregators;

        public RepositoryBuilder(ILogger<RepositoryBuilder> logger)
        {
            _logger = logger;
        }

        public async Task BuildUsingAggregators(bool useGamulator = true)
        {
            if (useGamulator)
            {
                var agg = new GamulatorAggregator();
                await agg.AggregateAllGames(_logger);
            }
        }
    }
}
