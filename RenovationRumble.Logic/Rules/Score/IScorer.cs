namespace RenovationRumble.Logic.Rules.Score
{
    using RenovationRumble.Logic.Runtime.Runner;

    /// <summary>
    /// Computes a score from the provided state.
    /// </summary>
    public interface IScorer
    {
        uint ComputeScore(in ReadOnlyContext context);
    }
}