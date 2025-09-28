namespace RenovationRumble.Logic.Runtime.Board
{
    using System.Collections.Generic;
    using Primitives;

    public interface IReadOnlyBoard
    {
        Coords Size { get; }
        
        bool IsFilled(Coords position);
        bool IsWithinBounds(Coords position);
        bool CollidesWithFilledCells(BitMatrix matrix, Coords origin, int? ignorePieceIndex = null);

        bool TryGetPlacedPiece(int index, out BoardPiece piece);
        BoardPiece GetPlacedPiece(int index);
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
        
        public bool CollidesWithFilledCells(BitMatrix matrix, Coords origin, int? ignorePieceIndex = null)
        {
            foreach (var (dx, dy) in matrix.FilledCells())
            {
                var x = origin.x + dx;
                var y = origin.y + dy;

                if (!IsWithinBounds(new Coords(x, y)))
                    return true;

                // Ignore a currently filled cell if it's part of the given piece
                if (fillMap[x, y])
                {
                    if (ignorePieceIndex.HasValue && TryGetPlacedPiece(ignorePieceIndex.Value, out var existingPiece))
                    {
                        var localX = x - existingPiece.coords.x;
                        var localY = y - existingPiece.coords.y;

                        if (localX >= 0 && localY >= 0 &&
                            localX < existingPiece.contents.w &&
                            localY < existingPiece.contents.h &&
                            existingPiece.contents[localX, localY])
                        {
                            continue;
                        }
                    }

                    return true;
                }
            }

            return false;
        }


        public bool TryGetPlacedPiece(int index, out BoardPiece piece)
        {
            if (index < 0 || index >= placedPieces.Count)
            {
                piece = default;
                return false;
            }
            
            piece = placedPieces[index];
            return true;
        }

        public BoardPiece GetPlacedPiece(int index)
        {
            return placedPieces[index];
        }

        public void Fill(Coords position, bool value = true)
        {
            fillMap[position.x, position.y] = value;
        }

        public void Fill(in BitMatrix matrix, Coords origin, bool value = true)
        {
            foreach (var (cx, cy) in matrix.FilledCells())
                fillMap[origin.x + cx, origin.y + cy] = value;
        }

        public void AddPiece(BoardPiece piece)
        {
            placedPieces.Add(piece);
        }
        
        public void ReplacePiece(int index, BoardPiece piece)
        {
            placedPieces[index] = piece;
        }
    }
}
