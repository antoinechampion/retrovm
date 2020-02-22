using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using RetroVm.Core;

namespace RetroVm.Server
{
    internal sealed class GamulatorAggregator : Aggregator
    {
        public GamulatorAggregator(string outPath)
        : base(
            "https://www.gamulator.com", 
            new Dictionary<string, string>
            {
                { "nes", "roms/nes" }
            },
            outPath
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
            var pageNum = Int32.Parse(regexResults.Groups[2].Value);
            return $"{regexResults.Groups[1].Value}{pageNum + 1}";
        }

        private Game ScrapSingleGame(HtmlNode node)
        {
            var currentGame = new Game();

            var gameName =
                node.SelectSingleNode(".//h5[@class='card-title']")
                    .InnerText
                    .Trim();
            currentGame.Name = RomTagCollection.RemoveTagsFromString(gameName);

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
                imgUri = imgUri.Replace(" ", "%");
                currentGame.ThumbnailUri = $"https://www.gamulator.com{imgUri}";
            }
            catch (NodeNotFoundException)
            {
                currentGame.ThumbnailUri = null;
            }

            var fileName =
                dlNode.SelectSingleNode("//td[text()[contains(., 'zip')]]")
                    .InnerText;
            RomTagCollection.TryAssignTags(currentGame, fileName);

            var dlUri =
                $"https://downloads.gamulator.com/roms/{fileName.Replace(" ", "%20")}";
            if (!RequestHead(dlUri).Result)
            {
                throw new FileNotFoundException();
            }

            currentGame.DownloadUri = dlUri;

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