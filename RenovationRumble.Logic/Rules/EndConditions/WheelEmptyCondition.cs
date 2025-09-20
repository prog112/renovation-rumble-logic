namespace RenovationRumble.Logic.Rules.EndConditions
{
    using Runtime.Runner;

    public sealed class WheelEmptyCondition : IEndCondition
    {
        public bool IsGameOver(in ReadOnlyContext context, out EndReason reason)
        {
            if (context.State.Wheel.IsEmpty)
            {
                reason = EndReason.WheelEmpty;
                return true;
            }

            reason = EndReason.None;
            return false;
        }
    }
}