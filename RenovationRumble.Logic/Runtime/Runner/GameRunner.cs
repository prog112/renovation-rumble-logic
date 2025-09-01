namespace RenovationRumble.Logic.Runtime.Runner
{
    using Board;
    using Data;
    using Data.Commands;
    using Executors;
    using Logger;
    using Primitives;

    public sealed class GameRunner
    {
        private readonly Context context;
        private readonly CommandRunner commandRunner;
        
        public GameRunner(ILogicLogger logicLogger = null)
        {
            context = new Context
            {
                Logger = logicLogger ?? NullLogger.Logger,
                RotationCache = new RotationCache()
            };
            
            commandRunner = new CommandRunner();
            commandRunner.Register(new PlaceExecutor());
        }

        public void StartMatch(MatchDataModel match)
        {
            // Create a new board
            context.Board = new Board(new Coords(match.BoardWidth, match.BoardHeight));
        }
        
        public CommandResult TryApplyCommand(CommandDataModel command)
        {
            return commandRunner.TryApplyCommand(context, command);
        }
    }
}