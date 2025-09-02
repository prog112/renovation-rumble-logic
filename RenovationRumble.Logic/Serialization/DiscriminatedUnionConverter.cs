namespace RenovationRumble.Logic.Serialization
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Polymorphic JSON for TBase using an enum discriminator and a factory map.
    /// IL2CPP-friendly and secure with no reflection under the hood.
    /// </summary>
    public sealed class DiscriminatedUnionConverter<TBase, TEnum> : JsonConverter<TBase>
        where TEnum : struct, Enum
        where TBase : class
    {
        private readonly string discriminatorName;
        private readonly IReadOnlyDictionary<TEnum, Func<TBase>> factories;
        private readonly Func<TBase, TEnum> getType;

        public DiscriminatedUnionConverter(
            string discriminatorName,
            IReadOnlyDictionary<TEnum, Func<TBase>> factories,
            Func<TBase, TEnum> getType)
        {
            this.discriminatorName = discriminatorName ?? throw new ArgumentNullException(nameof(discriminatorName));
            this.factories = factories ?? throw new ArgumentNullException(nameof(factories));
            this.getType = getType ?? throw new ArgumentNullException(nameof(getType));
        }

        public override void WriteJson(JsonWriter writer, TBase value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value is null) { writer.WriteNull(); return; }

            var enumValue = getType(value);

            // Resolve the runtime contract
            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(value.GetType());

            writer.WriteStartObject();

            // Discriminator goes first
            writer.WritePropertyName(discriminatorName);
            serializer.Serialize(writer, enumValue);

            // Then all properties per the contract
            foreach (var prop in contract.Properties)
            {
                if (prop.Ignored || !prop.Readable || (prop.ShouldSerialize != null && !prop.ShouldSerialize(value)))
                    continue;

                var propValue = prop.ValueProvider.GetValue(value);

                if (propValue == null && serializer.NullValueHandling == NullValueHandling.Ignore)
                    continue;

                writer.WritePropertyName(prop.PropertyName);
                
                // Use the prop's converter if present or fall back to the serializer
                if (prop.Converter != null)
                    prop.Converter.WriteJson(writer, propValue, serializer);
                else
                    serializer.Serialize(writer, propValue);
            }

            writer.WriteEndObject();
        }

        public override TBase ReadJson(JsonReader reader, Type objectType, TBase existingValue, bool hasExistingValue,
            Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jsonObject = JObject.Load(reader);
            if (!jsonObject.TryGetValue(discriminatorName, StringComparison.OrdinalIgnoreCase, out var token))
                throw new JsonSerializationException($"Missing '{discriminatorName}' discriminator.");

            TEnum type;
            switch (token.Type)
            {
                case JTokenType.String:
                {
                    var s = token.Value<string>();
                    if (!Enum.TryParse(typeof(TEnum), s, ignoreCase: true, out var parsed))
                        throw new JsonSerializationException($"Unknown {typeof(TEnum).Name} '{s}'.");
                    type = (TEnum)parsed!;
                    break;
                }
                case JTokenType.Integer:
                {
                    var i = token.Value<int>();
                    type = (TEnum)Enum.ToObject(typeof(TEnum), i);
                    break;
                }
                default:
                    throw new JsonSerializationException($"Invalid '{discriminatorName}' token type {token.Type}.");
            }

            if (!factories.TryGetValue(type, out var factory))
                throw new JsonSerializationException($"Unknown {typeof(TEnum).Name} '{type}'.");

            var instance = factory();
            using var jsonReader = jsonObject.CreateReader();
            serializer.Populate(jsonReader, instance); // Fill properties onto the created instance
            return instance;
        }
    }
}
