namespace RenovationRumble.Tests
{
    using System;
    using Logic.Primitives;

    internal static class BitMatrixTestHelpers
    {
        public static BitMatrix Build(params string[] rows)
        {
            if (rows is null || rows.Length == 0)
                throw new ArgumentException(null, nameof(rows));

            var h = (byte)rows.Length;
            var w = (byte)rows[0].Length;
            var bits = 0UL;

            for (int y = 0; y < h; y++)
            {
                if (rows[y].Length != w)
                    throw new ArgumentException("ragged rows");

                for (int x = 0; x < w; x++)
                {
                    if (rows[y][x] == '1')
                        bits |= 1UL << (y * w + x);
                }
            }

            return new BitMatrix(w, h, bits);
        }

        public static string[] ExtractRows(BitMatrix m)
        {
            var rows = new string[m.h];
            for (int y = 0; y < m.h; y++)
            {
                var chars = new char[m.w];
                for (int x = 0; x < m.w; x++)
                    chars[x] = m[x, y] ? '1' : '0';
                
                rows[y] = new string(chars);
            }

            return rows;
        }
    }
}