namespace RenovationRumble.Logic.Runtime
{
    using System.Collections.Generic;
    using Data;

    public sealed class Board
    {
        private readonly Coords size;

        private readonly List<BoardPiece> placedPieces;

        /// <summary>
        /// A helper grid array signifying which cell is already filled.
        /// </summary>
        private readonly Grid fillMap;

        public Board(Coords size)
        {
            this.size = size;

            placedPieces = new List<BoardPiece>();
            fillMap = new Grid(size.x, size.y);
        }
        
        public bool CanPlace(PieceDataModel data, Coords position, Orientation orientation)
        {
            var matrix = Rotate(data.DefaultContents, orientation);
            return Validate(matrix, position);
        }

        public bool TryPlace(PieceDataModel data, Coords position, Orientation orientation)
        {
            var matrix = Rotate(data.DefaultContents, orientation);

            // First pass: bounds + overlap
            if (!Validate(matrix, position))
                return false;

            // Second pass: cache the fill
            foreach (var (cx, cy) in matrix.FilledCells())
                fillMap[position.x + cx, position.y + cy] = true;

            placedPieces.Add(new BoardPiece(data, position, orientation, matrix));
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

        private static BitMatrix Rotate(in BitMatrix matrix, Orientation orientation)
        {
            // Potentially cache the precomputed rotations if it ever becomes an issue
            // (it really won't tho lol)
            return orientation switch
            {
                Orientation.Right => matrix.Rotate90(),
                Orientation.Bottom => matrix.Rotate180(),
                Orientation.Left => matrix.Rotate270(),
                _ => matrix
            };
        }
    }
}
