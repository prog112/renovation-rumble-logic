namespace RenovationRumble.Logic.Data.Commands
{
    using Primitives;

    public abstract class CommandDataModel
    {
    }

    public sealed class PlaceCommandDataModel : CommandDataModel
    {
        public ushort PieceId { get; set; }
        public Coords Coords { get; set; }
        public Orientation Orientation { get; set; }
    }

    public sealed class GrowCommandDataModel : CommandDataModel
    {
        public int PieceBoardIndex { get; set; }
        public Edge Edge { get; set; }
    }

    public sealed class ShrinkCommandDataModel : CommandDataModel
    {
        public int PieceBoardIndex { get; set; }
        public Edge Edge { get; set; }
    }
}