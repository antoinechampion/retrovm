using System;
using System.Collections.Generic;
using RetroVm.Core;

namespace RetroVm.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var l = new List<Source>
            {
                new Source
                {
                    Name = "default",
                    Mirrors = new List<string> { "alibaba.com/...", "azure.com/..."},
                    Supports = new List<string> {"atari st", "amiga"}
                },
                new Source
                {
                    Name = "homebrew-repo",
                    Mirrors = new List<string> {"mywebsite.com/..."},
                    Supports = new List<string> {"amiga"}
                }
            };
            YamlConfigurationFile.ToYaml("test.yml", l);
        }
    }
}
