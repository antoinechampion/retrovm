using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RetroVm.Core;

namespace RetroVm.Server
{
    internal abstract class Aggregator
    {
        protected string BaseUri { get; set; }
        protected Dictionary<string, string> Platforms { get; set; }

        protected Aggregator(string baseUri, Dictionary<string, string> platforms)
        {
            BaseUri = baseUri;
            Platforms = platforms;
        }

        protected abstract string GetNextPageUri(string currentPage);
        protected abstract IEnumerable<Game> GetGamesOnPage(string currentPageHtml);
        protected abstract string GetUriForPlatform(string platformName);

        public async Task<List<Game>> AggregateAllGames(ILogger logger)
        {
            var games = new List<Game>();

            foreach (var (platformName, platformPattern) in Platforms)
            {
                games.AddRange(
                    await AggregateAllGamesOnPlatform(platformName, logger)
                );
            }

            return games;
        }

        public async Task<List<Game>> AggregateAllGamesOnPlatform(string platformName, ILogger logger)
        {
            var games = new List<Game>();

            var uri = GetUriForPlatform(platformName);
            var currentPageHtml = await RequestGet(uri);

            do
            {
                var newGames =
                    GetGamesOnPage(currentPageHtml)
                        .Select(c =>
                        {
                            c.Platform = platformName;
                            return c;
                        })
                        .ToList();
                LogGamesInfo(logger, newGames);
                games.AddRange(newGames);
                uri = GetNextPageUri(uri);
                currentPageHtml = await RequestGet(uri);
            } while (currentPageHtml != null);

            return games;
        }

        protected void LogGamesInfo(ILogger logger, List<Game> games)
        {
            var logStr = $"GET {BaseUri}\n";
            foreach (var game in games)
            {
                logStr +=
                    $"{game.Platform?.ToUpper()}\t" +
                    $"{(game.Zone ?? "no zone found").ToUpper()}\t" +
                    $"{game.Name}\n";
            }

            logger.LogInformation(logStr);
        }

        public static string TryParseZone(string rawZoneText)
        {
            if ("europe,eu,pal,european"
                .Split(",")
                .Contains(rawZoneText, StringComparer.InvariantCultureIgnoreCase))
            {
                return "eu";
            }

            if (("usa,us,ntsc,america,united states,american")
                .Split(",")
                .Contains(rawZoneText, StringComparer.InvariantCultureIgnoreCase))
            {
                return "us";
            }

            if ("japan,jp,jap,japanese"
                .Split(",")
                .Contains(rawZoneText, StringComparer.InvariantCultureIgnoreCase))
            {
                return "jp";
            }

            return null;
        }

        protected async Task<string> RequestGet(string uri)
        {
            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using var response = (HttpWebResponse) await request.GetResponseAsync();
            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            await using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
            if (response.StatusCode == HttpStatusCode.OK)
                return await reader.ReadToEndAsync();
            else
                return null;
        }

        protected async Task<bool> RequestHead(string uri)
        {
            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = "HEAD";

            using var response = (HttpWebResponse) await request.GetResponseAsync();
            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
