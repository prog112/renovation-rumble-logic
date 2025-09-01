namespace RenovationRumble.Logic.Data.Commands
{
    using Primitives;

    public sealed class GrowCommandDataModel : CommandDataModel
    {
        public int PieceBoardIndex { get; set; }
        public Edge Edge { get; set; }
    }
}