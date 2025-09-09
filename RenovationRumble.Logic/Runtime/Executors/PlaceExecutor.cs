namespace RenovationRumble.Logic.Runtime.Executors
{
    using Board;
    using Data.Commands;
    using Primitives;
    using Runner;

    public sealed class PlaceExecutor : ICommandExecutor<PlaceCommandDataModel>
    {
        public bool CanApply(in ReadOnlyContext context, PlaceCommandDataModel command)
        {
            // Can we even retrieve a piece at this index?
            if (!context.State.Wheel.Peek(command.PieceWheelIndex, out var pieceId))
            {
                context.Logger.LogError($"Cannot access piece at index {command.PieceWheelIndex} in the wheel!");
                return false;
            }
            
            var piece = context.Data.GetPiece(pieceId);
            var matrix = context.Data.RotationCache.Get(piece, command.Orientation);
            var position = command.Position;

            foreach (var (cx, cy) in matrix.FilledCells())
            {
                var cell = new Coords(position.x + cx, position.y + cy);

                // Bounds
                if (!context.State.Board.IsWithinBounds(cell))
                {
                    context.Logger.LogError($"Cell {cell} is out of bounds!");
                    return false;
                }

                // Overlap
                if (context.State.Board.IsFilled(cell))
                {
                    context.Logger.LogError($"Cell {cell} is already occupied!");
                    return false;
                }
            }

            return true;
        }

        public void Apply(Context context, PlaceCommandDataModel command)
        {
            var pieceId = context.State.Wheel.Take(command.PieceWheelIndex);
            var piece = context.Data.GetPiece(pieceId);
            var matrix = context.Data.RotationCache.Get(piece, command.Orientation);
            var position = command.Position;

            context.State.Board.Fill(matrix, position);
            context.State.Board.AddPiece(new BoardPiece(piece, position, command.Orientation, matrix));
        }
    }
}