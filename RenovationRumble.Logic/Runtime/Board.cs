namespace RenovationRumble.Logic.Runtime
{
    using System.Collections.Generic;
    using Data;
    using Primitives;

    public sealed class Board
    {
        private readonly Coords size;
        private readonly List<BoardPiece> placedPieces;
        private readonly Grid fillMap; // A helper grid array signifying which cell is already filled

        private readonly RotationCache rotationCache;
        
        public Board(Coords size)
        {
            this.size = size;

            placedPieces = new List<BoardPiece>();
            fillMap = new Grid(size.x, size.y);
            rotationCache = new RotationCache();
        }
        
        public bool CanPlace(PieceDataModel piece, Coords position, Orientation orientation)
        {
            var matrix = rotationCache.Get(piece, orientation);
            return Validate(matrix, position);
        }

        public bool TryPlace(PieceDataModel piece, Coords position, Orientation orientation)
        {
            var matrix = rotationCache.Get(piece, orientation);

            // First pass: bounds + overlap
            if (!Validate(matrix, position))
                return false;

            // Second pass: cache the fill
            foreach (var (cx, cy) in matrix.FilledCells())
                fillMap[position.x + cx, position.y + cy] = true;

            placedPieces.Add(new BoardPiece(piece, position, orientation, matrix));
            return true;
        }

        private bool Validate(in BitMatrix matrix, in Coords position)
        {
            foreach (var (cx, cy) in matrix.FilledCells())
            {
                var gx = position.x + cx;
                var gy = position.y + cy;

                // Bounds
                if (gx >= size.x || gy >= size.y)
                    return false;

                // Overlap
                if (fillMap[gx, gy])
                    return false;
            }

            return true;
        }
    }
}
