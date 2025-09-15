namespace RenovationRumble.Logic.Rules.EndConditions
{
    using RenovationRumble.Logic.Primitives;
    using RenovationRumble.Logic.Runtime.Runner;

    public sealed class BoardFullCondition : IEndCondition
    {
        public bool IsGameOver(in ReadOnlyContext context, out EndReason reason)
        {
            var size = context.State.Board.Size;
            for (byte x = 0; x < size.y; x++)
            {
                for (byte y = 0; y < size.x; y++)
                {
                    if (!context.State.Board.IsFilled(new Coords(y, x)))
                    {
                        reason = EndReason.None;
                        return false;
                    }
                }
            }

            reason = EndReason.BoardFull;
            return true;
        }
    }
}