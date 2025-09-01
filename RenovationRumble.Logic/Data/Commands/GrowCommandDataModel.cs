namespace RenovationRumble.Logic.Data.Commands
{
    using Primitives;

    public sealed class GrowCommandDataModel : CommandDataModel
    {
        public override Command Command => Command.Grow;

        public int PieceBoardIndex { get; set; }
        public Edge Edge { get; set; }
    }
}