using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using YamlDotNet.Serialization;

namespace RetroVm.Core
{
    public class Game : ISerializable
    {
        public string Name { get; set; }
        public string DownloadUri { get; set; }
        public string ThumbnailBytes { get; set; }
        public string Platform { get; set; }
        [YamlIgnore]
        public ISet<string> ValidZones { get; } = new SortedSet<string> {"eu", "us", "jp"};

        private string _zone;
        public string Zone
        {
            get => _zone;
            set => _zone = ValidZones.Contains(value) ? value : null;
        }

        /// <inheritdoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", Name);
            info.AddValue("downloadUri", DownloadUri);
            info.AddValue("thumbnailBytes", ThumbnailBytes);
        }
    }
}
