using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;
using System.Text;
using RetroVm.Core.Exceptions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RetroVm.Core
{
    public sealed class YamlConfigurationFile
    {
        public static List<T> FromYaml<T>(string path) where T : ISerializable
        {
            List<T> t;
            try
            {
                t = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build()
                        .Deserialize<List<T>>(
                            File.ReadAllText(path)
                        );
            }
            catch (IOException ex)
            {
                throw new ConfigurationDeserializationException(
                    $"Can't load configuration file: {path}", ex);
            }
            catch (YamlDotNet.Core.YamlException ex)
            {
                throw new ConfigurationDeserializationException(
                    $"Malformed configuration file: {path}", ex);
            }
            return t;
        }

        public static void ToYaml<T>(string path, IEnumerable<T> content, bool append = false) 
            where T : ISerializable
        {
            try
            {
                var yaml = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build()
                    .Serialize(content);
                CreateParents(path);
                if (append && File.Exists(path))
                    File.AppendAllText(path, yaml);
                else
                    File.WriteAllText(path, yaml);
            }
            catch (IOException ex)
            {
                throw new ConfigurationDeserializationException(
                    $"Can't write configuration file: {path}", ex);
            }
        }
        
        private static void CreateParents(string path)
        {
            var filePath = Directory.GetParent(path).ToString();
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
        }
    }
}
