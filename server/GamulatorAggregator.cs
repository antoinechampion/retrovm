using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using RetroVm.Core;

namespace RetroVm.Server
{
    internal class GamulatorAggregator : Aggregator
    {
        public GamulatorAggregator()
        : base(
            "https://www.gamulator.com", 
            new Dictionary<string, string>
            {
                { "nes", "roms/nes" }
            }
        )
        { }


        /// <inheritdoc />
        protected override string GetUriForPlatform(string platformName)
        {
            return $"{BaseUri}/{Platforms[platformName]}?currentpage=1";
        }

        /// <inheritdoc />
        protected override string GetNextPageUri(string currentPage)
        {
            var pattern = $@"({BaseUri}/.*currentpage=)(\d+)";
            var regexResults = Regex.Match(currentPage, pattern);
            var pageNum = int.Parse(regexResults.Groups[2].Value);
            return $"{regexResults.Groups[1].Value}{pageNum + 1}";
        }

        private Game ScrapSingleGame(HtmlNode node)
        {
            var currentGame = new Game();

            currentGame.Name =
                node.SelectSingleNode(".//h5[@class='card-title']")
                    .InnerText;

            var downloadPageUri =
                BaseUri +
                node.SelectSingleNode(".//div[@class='card-body']//a")
                    .Attributes["href"]
                    .Value;

            var downloadHtml = RequestGet(downloadPageUri).Result;
            var downloadDoc = new HtmlDocument();
            downloadDoc.LoadHtml(downloadHtml);
            var dlNode = downloadDoc.DocumentNode;

            try
            {
                var imgUri =
                    dlNode.SelectSingleNode("//div[@class='margini']//picture//img")
                        .Attributes["src"]
                        .Value;
                using var webClient = new WebClient();
                var imageBytes = webClient.DownloadData($"{BaseUri}{imgUri}");
                currentGame.ThumbnailBytes = Convert.ToBase64String(imageBytes);
            }
            catch (NodeNotFoundException)
            {
                currentGame.ThumbnailBytes = null;
            }

            var fileName =
                dlNode.SelectSingleNode("//td[text()[contains(., 'zip')]]")
                    .InnerText;
            currentGame.DownloadUri =
                $"https://downloads.gamulator.com/roms/{fileName}";
            if (!RequestHead(currentGame.DownloadUri).Result)
            {
                throw new FileNotFoundException();
            }

            var location =
                dlNode.SelectSingleNode("//td[@itemprop='gameLocation']")
                    .InnerText;
            currentGame.Zone = TryParseZone(location) ?? "jp"; // Infer Japan

            return currentGame;
        }

        /// <inheritdoc />
        protected override IEnumerable<Game> GetGamesOnPage(string currentPageHtml)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(currentPageHtml);
            var gameNodes = doc.DocumentNode.SelectNodes("//div[@class='card']");
            foreach (var node in gameNodes)
            {
                Game currentGame;
                try
                {
                    currentGame = ScrapSingleGame(node);
                }
                catch
                {
                    continue;
                }
                yield return currentGame;
            }
        }
    }
}