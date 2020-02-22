using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Core.Tokens;

namespace RetroVm.Core
{
    public class RomTagCollection
    {
        public static RomTag Alternate1 { get; set; }
            = new RomTag
            {
                CommonName = "Alternate 1",
                Priority = -3,
                RegexPattern = new Regex(@"\[a1\]")
            };
        public static RomTag Alternate2 { get; set; }
            = new RomTag
            {
                CommonName = "Alternate 2",
                Priority = -2,
                RegexPattern = new Regex(@"\[a2\]")
            };
        public static RomTag Alternate3 { get; set; }
            = new RomTag
            {
                CommonName = "Alternate 3",
                Priority = -1,
                RegexPattern = new Regex(@"\[a3\]")
            };

        public static RomTag Pirate { get; set; }
            = new RomTag
            {
                CommonName = "Pirate",
                Priority = -5,
                RegexPattern = new Regex(@"\[p\d*\]")
            };

        public static RomTag BadDump { get; set; }
            = new RomTag
            {
                CommonName = "Bad Dump",
                Priority = -1000,
                RegexPattern = new Regex(@"\[b\d*\]")
            };
        public static RomTag Trained { get; set; }
            = new RomTag
            {
                CommonName = "Trained",
                Priority = -10,
                RegexPattern = new Regex(@"\[t\d*\]")
            };
        public static RomTag Fixed { get; set; }
            = new RomTag
            {
                CommonName = "Fixed",
                Priority = 10,
                RegexPattern = new Regex(@"\[f\d*\]")
            };
        public static RomTag OldTranslation { get; set; }
            = new RomTag
            {
                CommonName = "Old Translation",
                Priority = 0,
                RegexPattern = new Regex(@"\[T-\w+.*?\]")
            };
        public static RomTag NewerTranslation { get; set; }
            = new RomTag
            {
                CommonName = "Newer Translation",
                Priority = 0,
                RegexPattern = new Regex(@"\[T\+\w+.*?\]")
            };
        public static RomTag Hack { get; set; }
            = new RomTag
            {
                CommonName = "Hack",
                Priority = -20,
                RegexPattern = new Regex(@"\([^\(]*Hack[^\)]*\)|\[h\]")
            };
        public static RomTag UnknownYear { get; set; }
            = new RomTag
            {
                CommonName = "Unknown Year",
                Priority = -3,
                RegexPattern = new Regex(@"\(-\)")
            };
        public static RomTag Overdump { get; set; }
            = new RomTag
            {
                CommonName = "Overdump",
                Priority = -5,
                RegexPattern = new Regex(@"\[o\]")
            };
        public static RomTag VerifiedGoodDump { get; set; }
            = new RomTag
            {
                CommonName = "Verified Good Dump",
                Priority = 100,
                RegexPattern = new Regex(@"\[\!\]")
            };
        public static RomTag Multilanguage { get; set; }
            = new RomTag
            {
                CommonName = "Multilanguage",
                Priority = 20,
                RegexPattern = new Regex(@"\(M\d+\)")
            };


        /* Country Codes */
        public static RomTag JapanKorea { get; set; }
            = new RomTag
            {
                CommonName = "Japan & Korea",
                Priority = 0,
                RegexPattern = new Regex(@"\(1\)")
            };
        public static RomTag UsaBrazil { get; set; }
            = new RomTag
            {
                CommonName = "USA & Brazil - NTSC",
                Priority = 0,
                RegexPattern = new Regex(@"\(4\)")
            };
        public static RomTag Australia { get; set; }
            = new RomTag
            {
                CommonName = "Australia",
                Priority = 0,
                RegexPattern = new Regex(@"\(A\)")
            };
        public static RomTag Japan { get; set; }
            = new RomTag
            {
                CommonName = "Japan",
                Priority = 0,
                RegexPattern = new Regex(@"\(J\)")
            };
        public static RomTag Brazil { get; set; }
            = new RomTag
            {
                CommonName = "Brazil",
                Priority = 0,
                RegexPattern = new Regex(@"\(B\)")
            };
        public static RomTag Korea { get; set; }
            = new RomTag
            {
                CommonName = "Korea",
                Priority = 0,
                RegexPattern = new Regex(@"\(K\)")
            };
        public static RomTag China { get; set; }
            = new RomTag
            {
                CommonName = "China",
                Priority = 0,
                RegexPattern = new Regex(@"\(C\)")
            };
        public static RomTag Netherlands { get; set; }
            = new RomTag
            {
                CommonName = "Netherlands",
                Priority = 0,
                RegexPattern = new Regex(@"\((?:NL|H)\)")
            };
        public static RomTag Europe { get; set; }
            = new RomTag
            {
                CommonName = "Europe",
                Priority = 0,
                RegexPattern = new Regex(@"\((?:E|EU|Europe)\)")
            };
        public static RomTag France { get; set; }
            = new RomTag
            {
                CommonName = "France",
                Priority = 0,
                RegexPattern = new Regex(@"\(F\)")
            };
        public static RomTag Spain { get; set; }
            = new RomTag
            {
                CommonName = "Spain",
                Priority = 0,
                RegexPattern = new Regex(@"\(S\)")
            };
        public static RomTag FrenchCanadian { get; set; }
            = new RomTag
            {
                CommonName = "French Canadian",
                Priority = 0,
                RegexPattern = new Regex(@"\(FC\)")
            };
        public static RomTag Sweden { get; set; }
            = new RomTag
            {
                CommonName = "Sweden",
                Priority = 0,
                RegexPattern = new Regex(@"\(SW\)")
            };
        public static RomTag Finland { get; set; }
            = new RomTag
            {
                CommonName = "Finland",
                Priority = 0,
                RegexPattern = new Regex(@"\(FN\)")
            };
        public static RomTag Usa { get; set; }
            = new RomTag
            {
                CommonName = "USA",
                Priority = 0,
                RegexPattern = new Regex(@"\((?:U|USA)\)")
            };
        public static RomTag Germany { get; set; }
            = new RomTag
            {
                CommonName = "Germany",
                Priority = 0,
                RegexPattern = new Regex(@"\(G\)")
            };
        public static RomTag England { get; set; }
            = new RomTag
            {
                CommonName = "England",
                Priority = 0,
                RegexPattern = new Regex(@"\(UK\)")
            };
        public static RomTag Greece { get; set; }
            = new RomTag
            {
                CommonName = "Greece",
                Priority = 0,
                RegexPattern = new Regex(@"\(GR\)")
            };
        public static RomTag HongKong { get; set; }
            = new RomTag
            {
                CommonName = "Hong Kong",
                Priority = 0,
                RegexPattern = new Regex(@"\(HK\)")
            };
        public static RomTag Italy { get; set; }
            = new RomTag
            {
                CommonName = "Italy",
                Priority = 0,
                RegexPattern = new Regex(@"\(I\)")
            };

        public static List<RomTag> AllTags =>
            new List<RomTag>
            {
                Alternate1, Alternate2, Alternate3, Pirate, BadDump,
                Trained, Fixed, OldTranslation, NewerTranslation,
                Hack, UnknownYear, Overdump, VerifiedGoodDump,
                Multilanguage, JapanKorea, UsaBrazil, Australia,
                Japan, Brazil, Korea, China, Netherlands, Europe,
                France, Spain, FrenchCanadian, Sweden, Finland,
                Usa, Germany, England, Greece, HongKong, Italy
            };

        public static Regex AnyTag { get; } = new Regex(@"[\(\[].*?[\)\]]");

        public static void TryAssignTags(Game game, string strWithTags)
        {
            foreach (var tag in AllTags)
            {
                var matches = tag.RegexPattern.Matches(strWithTags);
                if (matches.Count > 0)
                {
                    game.Tags.AddRange(matches.Select(m => m.Value));
                }
            }
        }

        public static string RemoveTagsFromString(string strWithTags)
        {
            return AnyTag.Replace(strWithTags, "");
        }
    }
}
