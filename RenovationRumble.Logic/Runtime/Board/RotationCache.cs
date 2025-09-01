namespace RenovationRumble.Logic.Runtime.Board
{
    using System.Collections.Generic;
    using Data;
    using Primitives;

    public sealed class RotationCache
    {
        private readonly Dictionary<ushort, BitMatrix[]> cache = new Dictionary<ushort, BitMatrix[]>();

        public BitMatrix Get(PieceDataModel piece, Orientation orientation)
        {
            if (!cache.TryGetValue(piece.PieceId, out var rotations))
            {
                var matrix = piece.DefaultContents;
                rotations = new[] { matrix, matrix.Rotate90(), matrix.Rotate180(), matrix.Rotate270() };
                cache[piece.PieceId] = rotations;
            }

            return rotations[(int)orientation];
        }
    }
}