namespace RenovationRumble.Logic.Runtime.Runner
{
    using Board;
    using Catalog;
    using Data;
    using Data.Commands;
    using Executors;
    using Logger;
    using Primitives;
    using State;
    using Wheel;

    public sealed class GameRunner
    {
        private readonly Context context;
        private readonly CommandRunner commandRunner;
        
        public GameRunner(GameData gameData, ILogicLogger logicLogger = null)
        {
            context = new Context
            {
                Logger = logicLogger ?? NullLogger.Logger,
                Data = gameData 
            };
            
            commandRunner = new CommandRunner();
            
            // Use a Roselyn source generator to register all commands without reflection
            CommandExecutorRegistry.RegisterAll(commandRunner);
        }

        public void StartMatch(MatchDataModel match)
        {
            // Create a new game state
            context.State = new GameState
            {
                Board = new Board(new Coords(match.BoardWidth, match.BoardHeight)),
                Wheel = new ChoiceWheel(match.StartingWheelPieces)
            };
        }
        
        public CommandResult TryApplyCommand(CommandDataModel command)
        {
            return commandRunner.TryApplyCommand(context, command);
        }
    }
}