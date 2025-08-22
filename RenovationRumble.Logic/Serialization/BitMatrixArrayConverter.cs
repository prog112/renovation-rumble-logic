namespace RenovationRumble.Logic.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Board;

    /// <summary>
    /// Serializes BitMatrix as a JSON matrix: [[0,1,0],[1,1,1]]
    /// Also accepts booleans on read: [[false,true],[true,false]]
    /// </summary>
    public sealed class BitMatrixArrayConverter : JsonConverter<BitMatrix>
    {
        public override BitMatrix Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected start of matrix (array).");

            var rows = new List<List<int>>();

            // Read each row
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType != JsonTokenType.StartArray)
                    throw new JsonException("Expected row array.");

                var row = new List<int>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;

                    switch (reader.TokenType)
                    {
                        case JsonTokenType.Number:
                            var v = reader.GetInt32();
                            if (v != 0 && v != 1)
                                throw new JsonException("Matrix values must be 0 or 1.");

                            row.Add(v);
                            break;
                        case JsonTokenType.True:
                            row.Add(1);
                            break;

                        case JsonTokenType.False:
                            row.Add(0);
                            break;

                        default:
                            throw new JsonException("Matrix values must be numbers 0/1 or booleans.");
                    }
                }

                rows.Add(row);
            }

            if (rows.Count == 0) 
                throw new JsonException("Matrix must have at least one row.");
            
            var w = rows[0].Count;
            if (w == 0)
                throw new JsonException("Matrix rows must be non-empty.");

            for (var i = 1; i < rows.Count; i++)
            {
                if (rows[i].Count != w) 
                    throw new JsonException("All matrix rows must have equal length.");
            }

            var h = rows.Count;
            if ((long)w * h > BitMatrix.MaxSize) 
                throw new JsonException("BitMatrix supports up to 64 cells.");

            ulong packed = 0;
            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    if (rows[y][x] != 0)
                        packed |= 1UL << (y * w + x);
                }
            }

            return new BitMatrix(w, h, packed);
        }

        public override void Write(Utf8JsonWriter writer, BitMatrix value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            
            for (var y = 0; y < value.h; y++)
            {
                writer.WriteStartArray();
                for (var x = 0; x < value.w; x++)
                    writer.WriteNumberValue(value[x, y] ? 1 : 0);
               
                writer.WriteEndArray();
            }
            
            writer.WriteEndArray();
        }
    }
}