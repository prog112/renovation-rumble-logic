namespace RenovationRumble.Logic.Serialization
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Polymorphic JSON for TBase using an enum discriminator and a factory map.
    /// IL2CPP-friendly and secure with no reflection under the hood.
    /// </summary>
    public sealed class EnumDiscriminatedConverter<TBase, TEnum> : JsonConverter<TBase>
        where TEnum : struct, Enum
        where TBase : class
    {
        private readonly string discriminatorName;
        private readonly IReadOnlyDictionary<TEnum, Func<TBase>> factories;
        private readonly Func<TBase, TEnum> getType;

        public EnumDiscriminatedConverter(
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
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            var type = getType(value);
            var jsonObject = JObject.FromObject(value, serializer);
            jsonObject.AddFirst(new JProperty(discriminatorName, JToken.FromObject(type, serializer)));
            jsonObject.WriteTo(writer);
        }

        public override TBase ReadJson(JsonReader reader, Type objectType, TBase existingValue, bool hasExistingValue,
            Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jsonObject = JObject.Load(reader);
            if (!jsonObject.TryGetValue(discriminatorName, StringComparison.OrdinalIgnoreCase, out var token))
                throw new JsonSerializationException($"Missing '{discriminatorName}' discriminator.");

            var type = token.Type switch
            {
                JTokenType.String => (TEnum)Enum.Parse(typeof(TEnum), token.Value<string>(), ignoreCase: true),
                JTokenType.Integer => (TEnum)Enum.ToObject(typeof(TEnum), token.Value<int>()),
                _ => throw new JsonSerializationException($"Invalid '{discriminatorName}' token type {token.Type}.")
            };

            if (!factories.TryGetValue(type, out var factory))
                throw new JsonSerializationException($"Unknown {typeof(TEnum).Name} '{type}'.");

            var instance = factory();
            using var jsonReader = jsonObject.CreateReader();
            serializer.Populate(jsonReader, instance); // Fill properties onto the created instance
            return instance;
        }
    }
}