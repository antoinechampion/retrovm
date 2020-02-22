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
        public string ThumbnailUri { get; set; }
        public string Platform { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        

        /// <inheritdoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", Name);
            info.AddValue("downloadUri", DownloadUri);
            info.AddValue("thumbnailUri", ThumbnailUri);
        }
    }
}
