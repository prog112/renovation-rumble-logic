namespace RenovationRumble.Logic.Runtime.Board
{
    using System.Collections.Generic;
    using Primitives;

    public sealed class Board
    {
        private readonly Coords size;
        
        private readonly List<BoardPiece> placedPieces;
        private readonly Grid fillMap; // A helper grid array signifying which cell is already filled
        
        public Board(Coords size)
        {
            this.size = size;

            placedPieces = new List<BoardPiece>();
            fillMap = new Grid(size.x, size.y);
        }

        public bool IsFilled(Coords position)
        {
            return fillMap[position.x, position.y];
        }
        
        public bool IsWithinBounds(Coords position)
        {
            return position.x < size.x && position.y < size.y;
        }

        public void Fill(Coords position, bool value)
        {
            fillMap[position.x, position.y] = value;
        }

        public void Fill(in BitMatrix matrix, Coords origin)
        {
            foreach (var (cx, cy) in matrix.FilledCells())
                fillMap[origin.x + cx, origin.y + cy] = true;
        }

        public void AddPiece(BoardPiece piece)
        {
            placedPieces.Add(piece);
        }
    }
}
