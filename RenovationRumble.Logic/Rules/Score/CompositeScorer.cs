namespace RenovationRumble.Logic.Rules.Score
{
    using Runtime.Runner;

    public sealed class CompositeScorer : IScorer
    {
        private readonly IScorer[] scorers;
        
        public CompositeScorer(params IScorer[] scorers)
        {
            this.scorers = scorers;
        }

        public uint ComputeScore(in ReadOnlyContext context)
        {
            uint score = 0;
            foreach (var scorer in scorers)
                score += scorer.ComputeScore(context);

            return score;
        }
    }
}