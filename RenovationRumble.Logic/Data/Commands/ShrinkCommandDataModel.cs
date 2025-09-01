namespace RenovationRumble.Logic.Data.Commands
{
    using Primitives;

    public sealed class ShrinkCommandDataModel : CommandDataModel
    {
        public override Command Command => Command.Shrink;
        
        public int PieceBoardIndex { get; set; }
        public Edge Edge { get; set; }
    }
}