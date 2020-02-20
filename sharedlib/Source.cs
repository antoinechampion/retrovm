using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RetroVm.Core
{
    public class Source : ISerializable
    {
        public string Name { get; set; }
        public List<string> Mirrors { get; set; }
        public List<string> Supports { get; set; }

        /// <inheritdoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", Name);
            info.AddValue("mirrors", Mirrors);
            info.AddValue("supports", Name);
        }
    }
}
