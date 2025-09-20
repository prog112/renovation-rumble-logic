namespace RenovationRumble.Logic.Runtime.Runner
{
    public enum Status : byte
    {
        NotStarted = 0,
        InProgress,
        Ended,
        GameError,
    }

    public readonly struct GameStatus
    {
        public readonly Status status;
        public readonly CommandResult lastCommandResult;
        public readonly EndResult endResult;

        private GameStatus(Status status, CommandResult lastCommandResult = default, EndResult endResult = default)
        {
            this.status = status;
            this.lastCommandResult = lastCommandResult;
            this.endResult = endResult;
        }

        public static GameStatus NotStarted() => new GameStatus(Status.NotStarted);
        public static GameStatus InProgress(CommandResult lastCommandResult) => new GameStatus(Status.InProgress, lastCommandResult);
        public static GameStatus Ended(EndResult endResult, CommandResult lastCommandResult) => new GameStatus(Status.Ended, lastCommandResult, endResult);
        public static GameStatus Error(CommandResult lastCommandResult) => new GameStatus(Status.GameError, lastCommandResult);
    }
}