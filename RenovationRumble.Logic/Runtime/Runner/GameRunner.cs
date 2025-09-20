namespace RenovationRumble.Logic.Runtime.Runner
{
    using System;
    using Board;
    using Catalog;
    using Data;
    using Data.Commands;
    using Executors;
    using Logger;
    using Primitives;
    using Rules.EndConditions;
    using Rules.Score;
    using State;
    using Wheel;

    public sealed class GameRunner
    {
        private readonly Context context;
        private readonly CommandRunner commandRunner;

        private readonly IScorer scorer;
        private readonly IEndCondition endCondition;

        private CommandResult commandResult;
        private EndResult endResult;

        public GameRunner(GameData gameData, ILogicLogger logicLogger = null)
        {
            context = new Context
            {
                Logger = logicLogger ?? NullLogger.Logger,
                Data = gameData,
                State = new GameState
                {
                    Phase = GamePhase.NotStarted
                }
            };

            commandRunner = new CommandRunner();

            // TODO: Create those based on MatchDataModel and an enum/source generator to instantiate these
            scorer = new FilledCellsScorer();
            endCondition = new CompositeCondition(new BoardFullCondition(), new WheelEmptyCondition());

            // Use a Roselyn source generator to register all commands without reflection
            CommandExecutorRegistry.RegisterAll(commandRunner);
        }

        public void Start(MatchDataModel match)
        {
            // Create a new game state
            context.State = new GameState
            {
                Board = new Board(new Coords(match.BoardWidth, match.BoardHeight)),
                Wheel = new ChoiceWheel(match.StartingWheelPieces),
                Phase = GamePhase.InProgress,
                Moves = 0
            };

            commandResult = CommandResult.Ok();
            endResult = default;
        }

        public void Process(CommandDataModel command)
        {
            if (context.State.Phase != GamePhase.InProgress)
            {
                commandResult = CommandResult.Fail(CommandError.InvalidCommand, "Game is not in progress!");
                return;
            }

            commandResult = commandRunner.TryApplyCommand(context, command);
            if (commandResult.isSuccess)
                context.State.Moves++;
        }

        public GameStatus Tick()
        {
            var phase = context.State.Phase;
            if (phase == GamePhase.NotStarted)
                return GameStatus.NotStarted();

            if (!commandResult.isSuccess)
                return GameStatus.Error(commandResult);

            if (phase == GamePhase.Ended)
                return GameStatus.Ended(endResult, commandResult);
            
            var snapshot = new ReadOnlyContext(context);
            if (phase == GamePhase.InProgress && endCondition.IsGameOver(snapshot, out var reason))
            {
                endResult = new EndResult(scorer.ComputeScore(snapshot), reason);
                context.State.Phase = GamePhase.Ended;
                return GameStatus.Ended(endResult, commandResult);
            }

            return GameStatus.InProgress(commandResult);
        }
    }
}