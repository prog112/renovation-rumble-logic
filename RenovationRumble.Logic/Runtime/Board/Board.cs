namespace RenovationRumble.Logic.Runtime.Board
{
    using System.Collections.Generic;
    using Primitives;

    public interface IReadOnlyBoard
    {
        Coords Size { get; }
        bool IsFilled(Coords position);
        bool IsWithinBounds(Coords position);
    }
    
    public sealed class Board : IReadOnlyBoard
    {
        public Coords Size { get; }
        
        private readonly List<BoardPiece> placedPieces;
        private readonly Grid fillMap; // A helper grid array signifying which cell is already filled
        
        public Board(Coords size)
        {
            Size = size;
            placedPieces = new List<BoardPiece>();
            fillMap = new Grid(size.x, size.y);
        }

        public bool IsFilled(Coords position)
        {
            return fillMap[position.x, position.y];
        }
        
        public bool IsWithinBounds(Coords position)
        {
            return position.x < Size.x && position.y < Size.y;
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
