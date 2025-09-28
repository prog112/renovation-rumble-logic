namespace RenovationRumble.Logic.Runtime.Executors
{
    using Board;
    using Data.Commands;
    using Primitives;
    using Rules.BitMatrix;
    using Runner;

    public sealed class GrowExecutor : ICommandExecutor<GrowCommandDataModel>
    {
        public bool CanApply(in ReadOnlyContext context, GrowCommandDataModel command)
        {
            if (!context.State.Board.TryGetPlacedPiece(command.PieceBoardIndex, out var piece))
            {
                context.Logger.LogError($"Cannot access piece at index '{command.PieceBoardIndex}' in the board!");
                return false;
            }

            var position = piece.coords;
            var matrix = piece.contents;
            var boardSize = context.State.Board.Size;

            if (command.Edge.IsHorizontal())
            {
                if (!HasRoomToGrowColumn(position, matrix, boardSize, command.Edge))
                {
                    context.Logger.LogError($"Piece at index '{command.PieceBoardIndex}' has no room to grow horizontally!");
                    return false;               
                }
                
                if (!BitMatrixCapacity.CanGrowColumn(matrix.w, matrix.h))
                {
                    context.Logger.LogError($"Piece at index '{command.PieceBoardIndex}' would be too big horizontally!");
                    return false;
                }
            }

            if (command.Edge.IsVertical())
            {
                if (!HasRoomToGrowRow(position, matrix, boardSize, command.Edge))
                {
                    context.Logger.LogError($"Piece at index '{command.PieceBoardIndex}' has no room to grow vertically!");
                    return false;               
                }
                
                if (!BitMatrixCapacity.CanGrowRow(matrix.w, matrix.h))
                {
                    context.Logger.LogError($"Piece at index '{command.PieceBoardIndex}' would be too big vertically!");
                    return false;
                }
            }

            
            var newCoords = DetermineNewPiecePosition(command, piece);
            var newMatrix = matrix.Grow(command.Edge);
            if (context.State.Board.CollidesWithFilledCells(newMatrix, newCoords, command.PieceBoardIndex))
            {
                context.Logger.LogError($"Piece at index '{command.PieceBoardIndex}' would overlap other pieces after growing.");
                return false;
            }
            
            return true;
        }

        public void Apply(Context context, GrowCommandDataModel command)
        {
            var piece = context.State.Board.GetPlacedPiece(command.PieceBoardIndex);
            var matrix = piece.contents;
            
            // Clear the old piece
            context.State.Board.Fill(matrix, piece.coords, false);
            
            var newCoords = DetermineNewPiecePosition(command, piece);
            var newMatrix = matrix.Grow(command.Edge);
            
            // Replace the piece & apply the new matrix to the board
            var newPiece = new BoardPiece(piece.dataModel, newCoords, piece.orientation, newMatrix);
            context.State.Board.ReplacePiece(command.PieceBoardIndex, newPiece);
            context.State.Board.Fill(newMatrix, newCoords);
        }

        private static Coords DetermineNewPiecePosition(GrowCommandDataModel command, BoardPiece piece)
        {
            // Coords relate to the top left corner of the piece
            // No need to modify it if the bottom/right edge is grown
            return command.Edge switch
            {
                Edge.Left => new Coords(piece.coords.x - 1, piece.coords.y),
                Edge.Top => new Coords(piece.coords.x, piece.coords.y - 1),
                _ => piece.coords
            };
        }
        
        private static bool HasRoomToGrowColumn(Coords position, BitMatrix matrix, Coords boardSize, Edge edge)
        {
            return (edge == Edge.Left && position.x > 0) ||
                   (edge == Edge.Right && position.x + matrix.w < boardSize.x);
        }

        private static bool HasRoomToGrowRow(Coords position, BitMatrix matrix, Coords boardSize, Edge edge)
        {
            return (edge == Edge.Top && position.y > 0) ||
                   (edge == Edge.Bottom && position.y + matrix.h < boardSize.y);
        }
    }
}