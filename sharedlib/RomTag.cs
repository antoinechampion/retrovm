using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace RetroVm.Core
{
    public class RomTag
    {
        [YamlIgnore]
        public string CommonName { get; set; }
        [YamlIgnore]
        public Regex RegexPattern { get; set; }
        [YamlIgnore]
        public int Priority { get; set; }
        public string ShortName { get; set; }

        public RomTag()
        { }

        public RomTag(RomTag romTag)
        {
            CommonName = romTag.CommonName;
            RegexPattern = romTag.RegexPattern;
            Priority = romTag.Priority;
        }
    }
}
