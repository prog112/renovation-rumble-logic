namespace RenovationRumble.Logic.Rules.Score
{
    using Primitives;
    using Runtime.Runner;

    public sealed class FilledCellsScorer : IScorer
    {
        private const int PointsPerCell = 1;
        
        public uint ComputeScore(in ReadOnlyContext context)
        {
            uint score = 0;
            var size = context.State.Board.Size;
            for (byte x = 0; x < size.y; x++)
            {
                for (byte y = 0; y < size.x; y++)
                {
                    if (context.State.Board.IsFilled(new Coords(y, x)))
                        score += PointsPerCell;                 
                }
            }
            
            return score;
        }
    }
}