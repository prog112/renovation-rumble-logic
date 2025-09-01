namespace RenovationRumble.Logic.Data.Commands
{
    using Primitives;

    public sealed class ShrinkCommandDataModel : CommandDataModel
    {
        public int PieceBoardIndex { get; set; }
        public Edge Edge { get; set; }
    }
}