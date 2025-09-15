namespace RenovationRumble.Logic.Runtime.Runner
{
    using Rules.EndConditions;

    public enum ResultType : byte
    {
        NotStarted = 0,
        InProgress,
        Ended,
        GameError,
    }

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

    public readonly struct GameResult
    {
        public readonly ResultType resultType;
        public readonly CommandResult lastCommandResult;
        public readonly EndResult endResult;

        private GameResult(ResultType resultType, CommandResult lastCommandResult = default, EndResult endResult = default)
        {
            this.resultType = resultType;
            this.lastCommandResult = lastCommandResult;
            this.endResult = endResult;
        }

        public static GameResult NotStarted() => new GameResult(ResultType.NotStarted);
        public static GameResult InProgress(CommandResult lastCommandResult) => new GameResult(ResultType.InProgress, lastCommandResult);
        public static GameResult Ended(EndResult endResult, CommandResult lastCommandResult) => new GameResult(ResultType.Ended, lastCommandResult, endResult);
        public static GameResult Error(CommandResult lastCommandResult) => new GameResult(ResultType.GameError, lastCommandResult);
    }
}