﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using YamlDotNet.Serialization;

namespace RetroVm.Core
{
    public class Game : ISerializable
    {
        public string Name { get; set; }
        public List<GameMirror> Mirrors { get; set; }
        public string Platform { get; set; }
        [YamlIgnore]
        public List<string> AllTags => Mirrors.SelectMany(m => m.Tags).ToList();


        /// <inheritdoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", Name);
            info.AddValue("mirrors", Mirrors);
            info.AddValue("platform", Platform);
        }
    }
}
