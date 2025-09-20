namespace RenovationRumble.Logic.Rules.EndConditions
{
    using Runtime.Runner;

    public enum EndReason : byte
    {
        None = 0,
        BoardFull,
        WheelEmpty,
        TurnLimitReached,
        NoLegalMoves,
        Custom
    }

    /// <summary>
    /// Checks whether the match has ended and provides a reason if so.
    /// </summary>
    public interface IEndCondition
    {
        bool IsGameOver(in ReadOnlyContext context, out EndReason reason);
    }
}
