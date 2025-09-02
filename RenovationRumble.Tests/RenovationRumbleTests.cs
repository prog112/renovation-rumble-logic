namespace RenovationRumble.Tests
{
    using Logic.Data;
    using Logic.Data.Commands;
    using Logic.Primitives;
    using Newtonsoft.Json;
    using JsonSerializer = Logic.Serialization.JsonSerializer;

    public class RenovationRumbleTests
    {
        private static readonly JsonSerializer _Serializer = new(isHumanReadable: true);

        [Fact]
        public void SerializeDeserialize_PlaceCommand_RoundTrip()
        {
            var place = new PlaceCommandDataModel
            {
                PieceId = 42,
                Position = new Coords(2, 5),
                Orientation = Orientation.Right
            };

            var json = _Serializer.Serialize<CommandDataModel>(place);
            var back = _Serializer.Deserialize<CommandDataModel>(json);

            var typed = Assert.IsType<PlaceCommandDataModel>(back);
            Assert.Equal((ushort)42, typed.PieceId);
            Assert.Equal(new Coords(2, 5), typed.Position);
            Assert.Equal(Orientation.Right, typed.Orientation);
        }

        [Fact]
        public void SerializeDeserialize_Piece_RoundTrip()
        {
            var piece = new PieceDataModel
            {
                PieceId = 42,
                StyleId = 6,
                DefaultContents = new BitMatrix(5, 5, 0b11111)
            };

            var json = _Serializer.Serialize(piece);
            var back = _Serializer.Deserialize<PieceDataModel>(json);

            Assert.Equal((ushort)42, back.PieceId);
            Assert.Equal((ushort)6, back.StyleId);
            Assert.Equal((byte)5, back.DefaultContents.w);
            Assert.Equal((byte)5, back.DefaultContents.h);
            Assert.Equal(piece.DefaultContents.bits, back.DefaultContents.bits);

            // Randomly check a few cells
            Assert.True(back.DefaultContents[0,0]);
            Assert.True(back.DefaultContents[4,0]);
            Assert.False(back.DefaultContents[0,1]);
        }

        [Fact]
        public void Deserialize_MissingType_Throws()
        {
            const string json = "{ \"pieceId\":7 }";
            Assert.Throws<JsonSerializationException>(() => _Serializer.Deserialize<CommandDataModel>(json));
        }

        [Fact]
        public void Deserialize_UnknownType_Throws()
        {
            const string json = "{ \"type\":\"Nope\", \"pieceId\":7 }";
            Assert.Throws<JsonSerializationException>(() => _Serializer.Deserialize<CommandDataModel>(json));
        }
        
        [Theory]
        [InlineData("{ \"type\": \"Place\", \"pieceId\": 1, \"position\": { \"x\": 0, \"y\": 0 }, \"orientation\": 0 }")]
        [InlineData("{ \"type\": 0, \"pieceId\": 1, \"position\": { \"x\": 0, \"y\": 0 }, \"orientation\": 0 }")]
        public void Deserialize_StringAndIntDiscriminator_Works(string json)
        {
            var result = _Serializer.Deserialize<CommandDataModel>(json);
            Assert.IsType<PlaceCommandDataModel>(result);
        }
    }
}