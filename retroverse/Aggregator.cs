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

        private readonly string _outFilePath; 

        protected abstract string GetNextPageUri(string currentPage);
        protected abstract IEnumerable<Game> GetGamesOnPage(string currentPageHtml);
        protected abstract string GetUriForPlatform(string platformName);

        protected Aggregator(string baseUri, Dictionary<string, string> platforms, string outFilePath)
        {
            BaseUri = baseUri;
            Platforms = platforms;
            _outFilePath = outFilePath;
        }

        public async Task AggregateAllGames(ILogger logger)
        {
            foreach (var (platformName, _) in Platforms)
            {
                await AggregateAllGamesOnPlatform(platformName, logger);
            }
        }

        public async Task AggregateAllGamesOnPlatform(string platformName, ILogger logger)
        {
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
                YamlConfigurationFile.ToYaml(_outFilePath, newGames, append:true);

                uri = GetNextPageUri(uri);
                currentPageHtml = await RequestGet(uri);
            } while (currentPageHtml != null);
        }

        protected void LogGamesInfo(ILogger logger, List<Game> games)
        {
            var logStr = $"GET {BaseUri}\n";
            var maxLenName = games.Max(o => o.Name.Length);
            foreach (var game in games)
            {
                logStr += string.Format("|{0,10}|{1,"+maxLenName+"}|{2,20}\n",
                    game.Platform?.ToUpper(),
                    game.Name,
                    string.Join("", game.Tags));
            }

            logger.LogInformation(logStr);
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
