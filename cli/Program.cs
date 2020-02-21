using System;
using System.Collections.Generic;
using RetroVm.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
                    Platforms = new List<string> {"atari st", "amiga"}
                },
                new Source
                {
                    Name = "homebrew-repo",
                    Mirrors = new List<string> {"mywebsite.com/..."},
                    Platforms = new List<string> {"amiga"}
                }
            };
            YamlConfigurationFile.ToYaml("test.yml", l);
        }
    }
}
