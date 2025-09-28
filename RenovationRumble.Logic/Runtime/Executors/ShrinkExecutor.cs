namespace RenovationRumble.Logic.Runtime.Executors
{
    using Board;
    using Data.Commands;
    using Primitives;
    using Rules.BitMatrix;
    using Runner;

    public sealed class ShrinkExecutor : ICommandExecutor<ShrinkCommandDataModel>
    {
        public bool CanApply(in ReadOnlyContext context, ShrinkCommandDataModel command)
        {
            if (!context.State.Board.TryGetPlacedPiece(command.PieceBoardIndex, out var piece))
            {
                context.Logger.LogError($"Cannot access piece at index '{command.PieceBoardIndex}' in the board!");
                return false;
            }

            var matrix = piece.contents;

            if (command.Edge.IsHorizontal() && matrix.w <= 1)
            {
                context.Logger.LogError($"Piece at index '{command.PieceBoardIndex}' is only 1 tile wide and cannot be shrunk horizontally.");
                return false;
            }

            if (command.Edge.IsVertical() && matrix.h <= 1)
            {
                context.Logger.LogError($"Piece at index '{command.PieceBoardIndex}' is only 1 tile tall and cannot be shrunk vertically.");
                return false;
            }

            return true;
        }

        public void Apply(Context context, ShrinkCommandDataModel command)
        {
            var piece = context.State.Board.GetPlacedPiece(command.PieceBoardIndex);
            var matrix = piece.contents;

            // Clear the old piece
            context.State.Board.Fill(matrix, piece.coords, false);

            // Compute new matrix and updated coords (if applicable)
            var newMatrix = matrix.Shrink(command.Edge);
            var newCoords = DetermineNewPiecePosition(command, piece, matrix, newMatrix);

            // Replace and apply new piece
            var newPiece = new BoardPiece(piece.dataModel, newCoords, piece.orientation, newMatrix);
            context.State.Board.ReplacePiece(command.PieceBoardIndex, newPiece);
            context.State.Board.Fill(newMatrix, newCoords);
        }

        private static Coords DetermineNewPiecePosition(ShrinkCommandDataModel command, BoardPiece piece, BitMatrix oldMatrix, BitMatrix newMatrix)
        {
            var coords = piece.coords;

            if (command.Edge == Edge.Left && newMatrix.w < oldMatrix.w)
                return new Coords(coords.x + 1, coords.y);

            if (command.Edge == Edge.Top && newMatrix.h < oldMatrix.h)
                return new Coords(coords.x, coords.y + 1);

            return coords;
        }
    }
}