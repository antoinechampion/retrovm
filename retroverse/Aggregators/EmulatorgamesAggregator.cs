using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RetroVm.Core;
using RetroVm.Server;

namespace Retroverse.Aggregators
{
    internal sealed class EmulatorgamesAggregator : Aggregator
    {
        public EmulatorgamesAggregator(string outFolderPath)
        : base(
            "https://www.emulatorgames.net/",
            new Dictionary<string, string>
            {
                { "acorn-8-bit", "roms/acorn-8-bit/" },
                { "acorn-archimedes", "roms/acorn-archimedes/" },
                { "acorn-electron", "roms/acorn-electron/" },
                { "amiga-500", "roms/amiga-500/" },
                { "amstrad-cpc", "roms/amstrad-cpc/" },
                { "amstrad-gx4000", "roms/amstrad-gx4000/" },
                { "apple-i", "roms/apple-i/" },
                { "apple-ii", "roms/apple-ii/" },
                { "apple-ii-gs", "roms/apple-ii-gs/" },
                { "arcade", "roms/mame-037b11/" },
                { "atari-800", "roms/atari-800/" },
                { "atari-2600", "roms/atari-2600/" },
                { "atari-5200", "roms/atari-5200/" },
                { "atari-7800", "roms/atari-7800/" },
                { "atari-jaguar", "roms/atari-jaguar/" },
                { "atari-lynx", "roms/atari-lynx/" },
                { "atari-st", "roms/atari-st/" },
                { "bally-pro-arcade-astrocade", "roms/bally-pro-arcade-astrocade/" },
                { "bandai-wonderswan", "roms/wonderswan/" },
                { "bbc-micro", "roms/bbc-micro/" },
                { "camputers-lynx", "roms/camputers-lynx/" },
                { "capcom-play-system-1", "roms/capcom-play-system-1/" },
                { "capcom-play-system-2", "roms/capcom-play-system-2/" },
                { "capcom-play-system-3", "roms/capcom-play-system-3/" },
                { "casio-loopy", "roms/casio-loopy/" },
                { "casio-pv1000", "roms/casio-pv1000/" },
                { "colecovision", "roms/colecovision/" },
                { "colecovision-adam", "roms/colecovision-adam/" },
                { "commodore-64", "roms/commodore-64/" },
                { "commodore-max-machine", "roms/commodore-max-machine/" },
                { "commodore-pet", "roms/commodore-pet/" },
                { "commodore-plus4-c16", "roms/commodore-plus4-c16/" },
                { "commodore-vic20", "roms/commodore-vic20/" },
                { "dragon-data-dragon", "roms/dragon-data-dragon/" },
                { "elektronika-bk", "roms/elektronika-bk/" },
                { "emerson-arcadia-2001", "roms/emerson-arcadia-2001/" },
                { "entex-adventure-vision", "roms/entex-adventure-vision/" },
                { "epoch-super-cassette-vision", "roms/epoch-super-cassette-vision/" },
                { "fairchild-channel-f", "roms/fairchild-channel-f/" },
                { "funtech-super-acan", "roms/funtech-super-acan/" },
                { "galaksija", "roms/galaksija/" },
                { "gamepark-gp32", "roms/gamepark-gp32/" },
                { "gce-vectrex", "roms/gce-vectrex/" },
                { "hartung-game-master", "roms/hartung-game-master/" },
                { "intellivision", "roms/intellivision/" },
                { "interact-family-computer", "roms/interact-family-computer/" },
                { "kaypro-ii", "roms/kaypro-ii/" },
                { "luxor-abc-800", "roms/luxor-abc-800/" },
                { "magnavox-odyssey-2", "roms/magnavox-odyssey-2/" },
                { "mattel-aquarius", "roms/mattel-aquarius/" },
                { "memotech-mtx512", "roms/memotech-mtx512/" },
                { "microsoft-xbox", "roms/xbox/" },
                { "miles-gordon-sam-coupe", "roms/miles-gordon-sam-coupe/" },
                { "msx-2", "roms/msx-2/" },
                { "msx-computer", "roms/msx-computer/" },
                { "nec-pc-engine", "roms/turbografx-16/" },
                { "nec-super-grafx", "roms/super-grafx/" },
                { "nintendo-64", "roms/nintendo-64/" },
                { "nintendo-ds", "roms/nintendo-ds/" },
                { "nintendo-famicom-disk-system", "roms/nintendo-famicom-disk-system/" },
                { "nintendo-game-boy", "roms/gameboy/" },
                { "nintendo-game-boy-advance", "roms/gameboy-advance/" },
                { "nintendo-game-boy-color", "roms/gameboy-color/" },
                { "nintendo-gamecube", "roms/gamecube/" },
                { "nintendo-nes", "roms/nintendo/" },
                { "nintendo-pokemon-mini", "roms/nintendo-pokemon-mini/" },
                { "nintendo-super-nes", "roms/super-nintendo/" },
                { "nintendo-virtual-boy", "roms/nintendo-virtual-boy/" },
                { "nintendo-wii", "roms/nintendo-wii/" },
                { "nokia-n-gage", "roms/nokia-n-gage/" },
                { "pel-varazdin-orao", "roms/pel-varazdin-orao/" },
                { "philips-videopac", "roms/philips-videopac/" },
                { "rca-studio-ii", "roms/rca-studio-ii/" },
                { "robotron-z1013", "roms/robotron-z1013/" },
                { "sega-32x", "roms/sega-32x/" },
                { "sega-dreamcast", "roms/dreamcast/" },
                { "sega-game-gear", "roms/game-gear/" },
                { "sega-master-system", "roms/sega-master-system/" },
                { "sega-megadrive", "roms/sega-genesis/" },
                { "sega-pico", "roms/sega-pico/" },
                { "sega-saturn", "roms/sega-saturn/" },
                { "sega-sg1000", "roms/sega-sg1000/" },
                { "sega-super-control-station", "roms/sega-super-control-station/" },
                { "sega-visual-memory-system", "roms/sega-visual-memory-system/" },
                { "sharp-mz-700", "roms/sharp-mz-700/" },
                { "sharp-x68000", "roms/sharp-x68000/" },
                { "sinclair-zx81", "roms/sinclair-zx81/" },
                { "snk-neo-geo", "roms/neo-geo/" },
                { "snk-neo-geo-pocket", "roms/neo-geo-pocket/" },
                { "snk-neo-geo-pocket-color", "roms/neo-geo-pocket-color/" },
                { "sony-playstation", "roms/playstation/" },
                { "sony-playstation-2", "roms/playstation-2/" },
                { "sony-playstation-portable", "roms/playstation-portable/" },
                { "sufami-turbo", "roms/sufami-turbo/" },
                { "tandy-color-computer", "roms/tandy-color-computer/" },
                { "tangerine-oric", "roms/tangerine-oric/" },
                { "thomson-mo5", "roms/thomson-mo5/" },
                { "tiger-game-com", "roms/tiger-game-com/" },
                { "vtech-creativision", "roms/vtech-creativision/" },
                { "vtech-v-smile", "roms/vtech-v-smile/" },
                { "wang-vs", "roms/wang-vs/" },
                { "watara-supervision", "roms/watara-supervision/" },
                { "z-machine-infocom", "roms/z-machine-infocom/" },
                { "zx-spectrum", "roms/zx-spectrum/" },
            },
            outFolderPath
        )
        { }


        /// <inheritdoc />
        protected override string GetUriForPlatform(string platformName)
        {
            return $"{BaseUri}{Platforms[platformName]}";
        }

        /// <inheritdoc />
        protected override string GetNextPageUri(string currentPageUri, string platformName)
        {
            var pattern = $@"({BaseUri}.*)/(\d+)";
            var regexResults = Regex.Match(currentPageUri, pattern);
            string newPageUri;
            if (!regexResults.Success) { 
                newPageUri = $"{BaseUri}{Platforms[platformName]}2/";
            }
            else
            {
                var pageNum = int.Parse(regexResults.Groups[2].Value);
                newPageUri = $"{regexResults.Groups[1].Value}{pageNum + 1}/";
            }

            return newPageUri;
        }

        private Game ScrapSingleGame(HtmlNode node)
        {
            var currentGame = new Game();

            var gameName =
                node.SelectSingleNode(".//a//span[@class='eg-box-title']")
                    .InnerText
                    .Trim();
            var tagString = gameName;
            currentGame.Name = RomTagCollection.RemoveTagsFromString(gameName);

            var downloadPageUri =
                node.SelectSingleNode(".//a")
                    .Attributes["href"]
                    .Value;

            currentGame.ThumbnailUri =
                node.SelectSingleNode(".//a//picture//img")
                    .Attributes["data-src"]
                    .Value
                    .Replace(" ", "%20");

            var downloadHtml = RequestGet(downloadPageUri).Result;
            var downloadDoc = new HtmlDocument();
            downloadDoc.LoadHtml(downloadHtml);
            var dlNode = downloadDoc.DocumentNode;

            var idNode =
                dlNode.SelectSingleNode("//input[@name='rom_id']");
            var jsonResult = 
                RequestPost(
                    "https://www.emulatorgames.net/prompt/",
                    new Dictionary<string, string>
                    {
                        { "get_type", "rom" },
                        { "get_id", idNode.Attributes["value"].Value }
                    }
                )
                .Result
                .Replace("\\/", "/")
                .Replace(" ", "%20");

            var result = JsonConvert.DeserializeObject<List<string>>(jsonResult);
            var fileName = Regex.Match(result[0], "[^/]+$").Value;
            tagString += fileName;
            RomTagCollection.TryAssignTags(currentGame, tagString);

            currentGame.DownloadUri = result[0];

            return currentGame;
        }

        /// <inheritdoc />
        protected override IEnumerable<Game> GetGamesOnPage(string currentPageHtml)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(currentPageHtml);
            var gameNodes = doc.DocumentNode.SelectNodes("//ul[@class='eg-list']//li");
            if (gameNodes == null)
                yield break;

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