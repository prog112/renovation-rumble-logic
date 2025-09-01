namespace RenovationRumble.Logic.Data.Commands
{
    using Primitives;

    public sealed class PlaceCommandDataModel : CommandDataModel
    {
        public ushort PieceId { get; set; }
        public Coords Coords { get; set; }
        public Orientation Orientation { get; set; }
    }
}