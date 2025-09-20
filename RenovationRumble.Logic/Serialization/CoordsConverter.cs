namespace RenovationRumble.Logic.Serialization
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Primitives;

    /// <summary>
    /// JSON converter for Coords that keeps the struct attribute-free.
    /// </summary>
    public sealed class CoordsConverter : JsonConverter<Coords>
    {
        public override void WriteJson(JsonWriter writer, Coords value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteStartObject();
            
            writer.WritePropertyName("x"); 
            writer.WriteValue(value.x);
            
            writer.WritePropertyName("y"); 
            writer.WriteValue(value.y);
            
            writer.WriteEndObject();
        }

        public override Coords ReadJson(JsonReader reader, Type objectType, Coords existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default;

            var obj = JObject.Load(reader);

            var xToken = obj["x"] ?? throw new JsonSerializationException("Missing 'x' for Coords.");
            var yToken = obj["y"] ?? throw new JsonSerializationException("Missing 'y' for Coords.");

            // Allow ints & clamp to byte range for ease of use
            var x = (byte)xToken.Value<int>();
            var y = (byte)yToken.Value<int>();

            return new Coords(x, y);
        }
    }
}