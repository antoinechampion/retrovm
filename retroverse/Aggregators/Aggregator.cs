using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RetroVm.Core;

namespace Retroverse.Aggregators
{
    internal abstract class Aggregator
    {
        protected string BaseUri { get; set; }
        private string Host => new Uri(BaseUri).Host;

        /* ALL PLATFORMS
         new Dictionary<string, string>
            {
                { "acorn-8-bit", "" },
                { "acorn-archimedes", "" },
                { "acorn-electron", "" },
                { "amiga-500", "" },
                { "amstrad-cpc", "" },
                { "amstrad-gx4000", "" },
                { "apple-i", "" },
                { "apple-ii", "" },
                { "apple-ii-gs", "" },
                { "arcade", "" },
                { "atari-800", "" },
                { "atari-2600", "" },
                { "atari-5200", "" },
                { "atari-7800", "" },
                { "atari-jaguar", "" },
                { "atari-lynx", "" },
                { "atari-st", "" },
                { "bally-pro-arcade-astrocade", "" },
                { "bandai-wonderswan", "" },
                { "bbc-micro", "" },
                { "camputers-lynx", "" },
                { "capcom-play-system-1", "" },
                { "capcom-play-system-2", "" },
                { "capcom-play-system-3", "" },
                { "casio-loopy", "" },
                { "casio-pv1000", "" },
                { "colecovision", "" },
                { "colecovision-adam", "" },
                { "commodore-64", "" },
                { "commodore-max-machine", "" },
                { "commodore-pet", "" },
                { "commodore-plus4-c16", "" },
                { "commodore-vic20", "" },
                { "dragon-data-dragon", "" },
                { "elektronika-bk", "" },
                { "emerson-arcadia-2001", "" },
                { "entex-adventure-vision", "" },
                { "epoch-super-cassette-vision", "" },
                { "fairchild-channel-f", "" },
                { "funtech-super-acan", "" },
                { "galaksija", "" },
                { "gamepark-gp32", "" },
                { "gce-vectrex", "" },
                { "hartung-game-master", "" },
                { "intellivision", "" },
                { "interact-family-computer", "" },
                { "kaypro-ii", "" },
                { "luxor-abc-800", "" },
                { "magnavox-odyssey-2", "" },
                { "mattel-aquarius", "" },
                { "memotech-mtx512", "" },
                { "microsoft-xbox", "" },
                { "miles-gordon-sam-coupe", "" },
                { "msx-2", "" },
                { "msx-computer", "" },
                { "nec-pc-engine", "" },
                { "nec-super-grafx", "" },
                { "neo-geo-pocket-color", "" },
                { "nintendo-64", "" },
                { "nintendo-ds", "" },
                { "nintendo-famicom-disk-system", "" },
                { "nintendo-game-boy", "" },
                { "nintendo-game-boy-advance", "" },
                { "nintendo-game-boy-color", "" },
                { "nintendo-gamecube", "" },
                { "nintendo-nes", "" },
                { "nintendo-pokemon-mini", "" },
                { "nintendo-super-nes", "" },
                { "nintendo-virtual-boy", "" },
                { "nintendo-wii", "" },
                { "nokia-n-gage", "" },
                { "pel-varazdin-orao", "" },
                { "philips-videopac", "" },
                { "rca-studio-ii", "" },
                { "robotron-z1013", "" },
                { "sega-32x", "" },
                { "sega-dreamcast", "" },
                { "sega-game-gear", "" },
                { "sega-master-system", "" },
                { "sega-mega-cd", "" },
                { "sega-megadrive", "" },
                { "sega-naomi", "" },
                { "sega-pico", "" },
                { "sega-saturn", "" },
                { "sega-sg1000", "" },
                { "sega-super-control-station", "" },
                { "sega-visual-memory-system", "" },
                { "sharp-mz-700", "" },
                { "sharp-x68000", "" },
                { "sinclair-zx81", "" },
                { "snk-neo-geo", "" },
                { "snk-neo-geo-pocket", "" },
                { "sony-playstation", "" },
                { "sony-playstation-2", "" },
                { "sony-playstation-portable", "" },
                { "sufami-turbo", "" },
                { "tandy-color-computer", "" },
                { "tangerine-oric", "" },
                { "thomson-mo5", "" },
                { "tiger-game-com", "" },
                { "vtech-creativision", "" },
                { "vtech-v-smile", "" },
                { "wang-vs", "" },
                { "watara-supervision", "" },
                { "z-machine-infocom", "" },
                { "zx-spectrum", "" },
            }
            */
        protected Dictionary<string, string> Platforms { get; set; }

        private readonly string _outFolderPath; 

        protected abstract string GetNextPageUri(string currentPageUri, string platformName);
        protected abstract IEnumerable<Game> GetGamesOnPage(string currentPageHtml);
        protected abstract string GetUriForPlatform(string platformName);

        protected Aggregator(string baseUri, Dictionary<string, string> platforms, string outFolderPath)
        {
            BaseUri = baseUri;
            Platforms = platforms;
            _outFolderPath = outFolderPath;
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
            var outFilePath = Path.Combine(_outFolderPath, $"{Host}-{platformName}.yml");
            var uri = GetUriForPlatform(platformName);
            var currentPageHtml = await RequestGet(uri);
            if (currentPageHtml == null)
                return;

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
                if (newGames.Count == 0)
                {
                    break;
                }
                LogGamesInfo(logger, newGames, uri);
                YamlConfigurationFile.ToYaml(outFilePath, newGames, append:true);

                uri = GetNextPageUri(uri, platformName);
                currentPageHtml = await RequestGet(uri);
            } while (currentPageHtml != null);
        }

        protected void LogGamesInfo(ILogger logger, List<Game> games, string uri)
        {
            var logStr = $"GET {uri}\n";
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
            if (uri == null)
                return null;
            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse) await request.GetResponseAsync();
            }
            catch
            {
                return null;
            }
            if ((int)response.StatusCode < 200 || (int)response.StatusCode > 309)
                return null;

            await using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
            if ((int) response.StatusCode < 200 || (int) response.StatusCode > 309)
                return null;
            else
                return await reader.ReadToEndAsync();
        }

        protected async Task<string> RequestPost(string uri, Dictionary<string, string> data)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("referer", BaseUri);

            var content = new FormUrlEncodedContent(data);
            var response = await client.PostAsync(uri, content);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
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
