using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using RetroVm.Core;
using RetroVm.Server;

namespace Retroverse.Aggregators
{
    internal sealed class GamulatorAggregator : Aggregator
    {
        public GamulatorAggregator(string outFolderPath)
        : base(
            "https://www.gamulator.com", 
            new Dictionary<string, string>
            {
                { "arcade", "roms/mame" },
                { "atari-2600", "roms/atari-2600" },
                { "atari-5200", "roms/atari-5200" },
                { "atari-7800", "roms/atari-7800" },
                { "atari-jaguar", "roms/atari-jaguar" },
                { "atari-lynx", "roms/atari-lynx" },
                { "capcom-play-system-2", "roms/capcom-play-system-2" },
                { "nintendo-game-boy", "roms/game-boy" },
                { "nintendo-game-boy-advance", "roms/game-boy-advance" },
                { "nintendo-game-boy-color", "roms/game-boy-color" },
                { "nec-pc-engine", "roms/turbografx16" },
                { "nintendo-64", "roms/nintendo-64" },
                { "nintendo-ds", "roms/nintendo-ds" },
                { "nintendo-gamecube", "roms/gamecube" },
                { "nintendo-nes", "roms/nes" },
                { "nintendo-super-nes", "roms/snes" },
                { "nintendo-virtual-boy", "roms/virtual-boy" },
                { "nintendo-wii", "roms/nintendo-wii" },
                { "sega-32x", "roms/sega-32x" },
                { "sega-dreamcast", "roms/sega-dreamcast" },
                { "sega-game-gear", "roms/sega-game-gear" },
                { "sega-master-system", "roms/sega-master-system" },
                { "sega-mega-cd", "roms/sega-cd" },
                { "sega-megadrive", "roms/sega-genesis" },
                { "sega-naomi", "roms/sega-naomi" },
                { "sega-saturn", "roms/sega-saturn" },
                { "snk-neo-geo", "roms/new-geo" },
                { "snk-neo-geo-pocket", "roms/neo-geo-pocket" },
                { "sony-playstation", "roms/psx" },
                { "sony-playstation-2", "roms/playstation-2" },
                { "sony-playstation-portable", "roms/psp" }
            },
            outFolderPath
        )
        { }


        /// <inheritdoc />
        protected override string GetUriForPlatform(string platformName)
        {
            return $"{BaseUri}/{Platforms[platformName]}?currentpage=1";
        }

        /// <inheritdoc />
        protected override string GetNextPageUri(string currentPageUri, string platformName)
        {
            var pattern = $@"({BaseUri}/.*currentpage=)(\d+)";
            var regexResults = Regex.Match(currentPageUri, pattern);
            var pageNum = Int32.Parse(regexResults.Groups[2].Value);
            var newPageUri = $"{regexResults.Groups[1].Value}{pageNum + 1}";

            /* Gamulator doesn't 404 if the next page doesn't exists
             but redirects instead to the last existing page */
            try
            {
                var webGet = new HtmlWeb();
                var document = webGet.Load(newPageUri);
                var title = document.DocumentNode.SelectSingleNode("html/head/title").InnerText;
                var newPageNum = int.Parse(Regex.Match(title, @"Page (\d+)").Groups[1].Value);
                if (newPageNum == pageNum)
                    newPageUri = null;
            } catch { newPageUri = null; }

            return newPageUri;
        }

        private Game ScrapSingleGame(HtmlNode node)
        {
            var currentGame = new Game();

            var gameName =
                node.SelectSingleNode(".//h5[@class='card-title']")
                    .InnerText
                    .Trim();
            var tagString = gameName;
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
                dlNode.SelectSingleNode("//td[text()[contains(., 'zip') or contains(., '7z') or contains(., 'rar')]]")
                    .InnerText;
            tagString += fileName;
            RomTagCollection.TryAssignTags(currentGame, tagString);

            currentGame.DownloadUri =
                $"https://downloads.gamulator.com/roms/{fileName.Replace(" ", "%20")}";

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