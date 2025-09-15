namespace RenovationRumble.Logic.Runtime.Runner
{
    public enum ResultType : byte
    {
        NotStarted = 0,
        InProgress,
        Ended,
        GameError,
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