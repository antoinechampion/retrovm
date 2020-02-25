using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Retroverse.Aggregators;

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

        public void BuildUsingAggregators(
            string outRepoPath, 
            bool useGamulator = true,
            bool useRomsmode = true,
            bool useEmulatorgames = true)
        {
            var outFolderPath = Path.Combine(outRepoPath, "aggregated/");
            _aggregators.Clear();

            if (useGamulator)
            {
                _aggregators.Add(new GamulatorAggregator(outFolderPath));
            }
            if (useRomsmode)
            {
                _aggregators.Add(new RomsmodeAggregator(outFolderPath));
            }
            if (useEmulatorgames)
            {
                _aggregators.Add(new EmulatorgamesAggregator(outFolderPath));
            }

            _logger.LogInformation("Starting aggregation...");

            // Task.WaitAll(_aggregators.Select(aggregator => aggregator.AggregateAllGames(_logger)).ToArray());

            CreateRepo(outFolderPath, outRepoPath);
        }


        public void CreateRepo(string aggregatedFolderPath, string repoPath)
        {
            var allPlatforms = 
            MapReduce
                .LoadAggregated(aggregatedFolderPath)
                .IterateByPlatform()
                .Select(o => o.RegroupMirrors());

            foreach (var platform in allPlatforms)
            {
                platform.ExportYaml(repoPath);
            }
        }
    }
}
