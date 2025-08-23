namespace RenovationRumble.Logic.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Newtonsoft-backed serializer with safe defaults for Unity/IL2CPP.
    /// </summary>
    public sealed class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings settings;

        private static readonly Encoding _Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        /// <param name="allowPolymorphism">If true, enables TypeNameHandling.Auto (ONLY for trusted data).</param>
        /// <param name="isHumanReadable">Pretty print for debugging.</param>
        public JsonSerializer(bool allowPolymorphism = false, bool isHumanReadable = false)
        {
            settings = new JsonSerializerSettings
            {
                // SECURITY: keep None for untrusted data
                TypeNameHandling = allowPolymorphism ? TypeNameHandling.Auto : TypeNameHandling.None,
                Formatting = isHumanReadable ? Formatting.Indented : Formatting.None,
                ContractResolver = new WritablePropertiesOnlyResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            // Global converters
            settings.Converters.Add(new BitMatrixArrayConverter());
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