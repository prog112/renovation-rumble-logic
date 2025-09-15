namespace RenovationRumble.Logic.Rules.EndConditions
{
    using RenovationRumble.Logic.Runtime.Runner;

    public sealed class CompositeCondition : IEndCondition
    {
        private readonly IEndCondition[] checks;

        public CompositeCondition(params IEndCondition[] checks)
        {
            this.checks = checks;
        }

        public bool IsGameOver(in ReadOnlyContext context, out EndReason reason)
        {
            foreach (var check in checks)
            {
                if (check.IsGameOver(in context, out reason))
                    return true;
            }

            reason = EndReason.None;
            return false;
        }
    }
}