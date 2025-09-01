namespace RenovationRumble.Logic.Data.Commands
{
    using Primitives;

    public sealed class PlaceCommandDataModel : CommandDataModel
    {
        public override Command Command => Command.Place;
        
        public ushort PieceId { get; set; }
        public Coords Position { get; set; }
        public Orientation Orientation { get; set; }
    }
}