namespace RenovationRumble.Logic.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Data.Commands;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Newtonsoft-backed serializer with safe defaults for Unity/IL2CPP.
    /// </summary>
    public sealed class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings settings;

        private static readonly Encoding _Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        
        public JsonSerializer(bool isHumanReadable = false)
        {
            settings = new JsonSerializerSettings
            {
                // Keep None for untrusted data for security
                TypeNameHandling = TypeNameHandling.None,
                Formatting = isHumanReadable ? Formatting.Indented : Formatting.None,
                ContractResolver = new WritablePropertiesOnlyResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            // Global converters
            settings.Converters.Add(new StringEnumConverter());
            settings.Converters.Add(new BitMatrixArrayConverter());
            
            // Handle polymorphism with a safe IL2CPP-friendly converter
            DiscriminatedUnionRegistry.RegisterAll(settings);
        }

        public string Serialize<T>(T data)
        {
            return JsonConvert.SerializeObject(data, settings);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, settings)!;
        }

        public byte[] SerializeBytes<T>(T data)
        {
            return _Utf8.GetBytes(Serialize(data));
        }

        public T DeserializeBytes<T>(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                throw new ArgumentException("Cannot deserialize empty payload.", nameof(bytes));
            
            var json = _Utf8.GetString(bytes);
            return Deserialize<T>(json);
        }
    }

    public class WritablePropertiesOnlyResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            return properties.Where(p => p.Writable).ToList();
        }
    }
}