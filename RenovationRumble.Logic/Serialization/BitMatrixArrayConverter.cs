namespace RenovationRumble.Logic.Serialization
{
    using System;
    using Data;
    using Data.Matrix;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Newtonsoft converter for BitMatrix. JSON shape: [[0,1,0],[1,1,1]] with booleans allowed.
    /// </summary>
    public class BitMatrixArrayConverter : JsonConverter<BitMatrix>
    {
        public override BitMatrix ReadJson(JsonReader reader, Type objectType, BitMatrix existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartArray)
                throw new JsonSerializationException("Expected start of matrix (array).");

            var root = JArray.Load(reader);
            if (root.Count == 0)
                throw new JsonSerializationException("Matrix must have at least one row.");

            var w = (root[0] as JArray)?.Count ?? 0;
            if (w == 0)
                throw new JsonSerializationException("Matrix rows must be non-empty.");

            // Validate all rows length
            for (var i = 0; i < root.Count; i++)
            {
                if (!(root[i] is JArray row) || row.Count != w)
                    throw new JsonSerializationException("All matrix rows must have equal length.");
            }

            var h = root.Count;
            if ((long)w * h > BitMatrix.MaxCells)
                throw new JsonSerializationException("BitMatrix supports up to 64 cells.");

            var bits = 0UL;
            for (var y = 0; y < h; y++)
            {
                var row = (JArray)root[y];
                for (var x = 0; x < w; x++)
                {
                    var token = row[x];

                    var value = token.Type switch
                    {
                        JTokenType.Integer => (int)token,
                        JTokenType.Boolean => token.Value<bool>() ? 1 : 0,
                        _ => throw new JsonSerializationException("Matrix values must be numbers 0/1 or booleans.")
                    };

                    if (value != 0 && value != 1)
                        throw new JsonSerializationException("Matrix values must be 0 or 1.");

                    if (value != 0)
                        bits |= 1UL << (y * w + x);
                }
            }

            return new BitMatrix((byte)w, (byte)h, bits);
        }

        public override void WriteJson(JsonWriter writer, BitMatrix value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteStartArray();
     
            for (var y = 0; y < value.h; y++)
            {
                writer.WriteStartArray();
            
                for (var x = 0; x < value.w; x++)
                    writer.WriteValue(value[x, y] ? 1 : 0);
                
                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }
    }
}