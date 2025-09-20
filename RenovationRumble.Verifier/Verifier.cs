namespace RenovationRumble.Verifier
{
    using Data;
    using Logic.Data;
    using Logic.Runtime.Catalog;
    using Logic.Runtime.Runner;

    internal static class Verifier
    {
        public static VerifyResponseDataModel Verify(CatalogDataModel catalog, VerifyRequestDataModel request)
        {
            if (request?.Match == null || request.Commands == null || request.Commands.Length == 0)
            {
                return new VerifyResponseDataModel
                {
                    Status = VerifyStatus.InvalidInput,
                    Message = "Missing match or commands."
                };
            }

            var gameData = new GameData(catalog);
            var runner = new GameRunner(gameData);
            runner.Start(request.Match);

            foreach (var command in request.Commands)
            {
                runner.Process(command);
                var status = runner.Tick();
                
                var response = TryComplete(request, status);
                if (response != null)
                    return response;
            }

            return new VerifyResponseDataModel
            {
                Status = VerifyStatus.DidNotEnd,
                Message = "Game did not reach an end state after all commands were processed."
            };
        }

        private static VerifyResponseDataModel TryComplete(VerifyRequestDataModel request, GameStatus gameStatus)
        {
            switch (gameStatus.status)
            {
                case Status.GameError:
                    return new VerifyResponseDataModel
                    {
                        Status = VerifyStatus.SimulationError,
                        Message = $"Command failed: {gameStatus.lastCommandResult.error} {gameStatus.lastCommandResult.message}"
                    };

                case Status.Ended:
                    var score = gameStatus.endResult.score;
                    if (score != request.ClaimedScore)
                    {
                        return new VerifyResponseDataModel
                        {
                            Status = VerifyStatus.ScoreMismatch,
                            Message = "Claimed score does not match computed score.",
                            ComputedScore = score,
                            EndReason = gameStatus.endResult.endReason.ToString()
                        };
                    }

                    return new VerifyResponseDataModel
                    {
                        Status = VerifyStatus.Ok,
                        Message = "Verified.",
                        ComputedScore = score,
                        EndReason = gameStatus.endResult.endReason.ToString()
                    };

                case Status.NotStarted:
                    return new VerifyResponseDataModel
                    {
                        Status = VerifyStatus.InvalidInput,
                        Message = "Game not started."
                    };

                case Status.InProgress:
                default:
                    return null; // Keep going
            }
        }
    }
}