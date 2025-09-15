namespace RenovationRumble.Logic.Runtime.Runner
{
    using Rules.EndConditions;

    public readonly struct EndResult
    {
        public readonly uint score;
        public readonly EndReason endReason;

        public EndResult(uint score, EndReason endReason)
        {
            this.score = score;
            this.endReason = endReason;
        }
    }
}