using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RetroVm.Core;

namespace Retroverse.Aggregators
{
    internal static class MapReduce
    {
        public static IEnumerable<GameMirror> LoadAggregated(string aggFolderPath)
        {
            var mirrors = Directory
                .EnumerateFiles(aggFolderPath, "*.yml")
                .SelectMany(YamlConfigurationFile.FromYaml<GameMirror>);

            foreach (var mirror in mirrors)
            {
                yield return mirror;
            }
        }

        public static IEnumerable<IEnumerable<GameMirror>> IterateByPlatform(this IEnumerable<GameMirror> mirrors)
        {
            var groups = mirrors.GroupBy(m => m.Platform);

            foreach (var group in groups)
            {
                yield return group;
            }
        }

        public static IEnumerable<Game> RegroupMirrors(this IEnumerable<GameMirror> mirrors)
        {
            var groups = 
                mirrors
                .GroupBy(m => m.Name)
                .Select(grp => new Game()
                {
                    Mirrors = grp.ToList(),
                    Name = grp.Key,
                    Platform = grp.First().Platform
                });
            foreach (var group in groups)
            {
                yield return group;
            }
        }

        public static void ExportYaml(this IEnumerable<Game> games, string repoPath)
        {
            // To avoid multiple enumeration
            var gamesList = games.ToList();

            var platform = gamesList.First().Platform;
            var exportPath = Path.Combine(repoPath, platform + ".yml");
            YamlConfigurationFile.ToYaml(exportPath, gamesList);
        }

    }
}
