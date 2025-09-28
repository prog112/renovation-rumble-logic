namespace RenovationRumble.Logic.Rules.BitMatrix
{
    using RenovationRumble.Logic.Primitives;

    /// <summary>
    /// Extensions that don't fit the low-level base <see cref="BitMatrix"/> class that is supposed to be design-agnostic.
    /// These methods below are more particular to the game itself.
    /// </summary>
    public static class BitMatrixResizing
    {
        public static BitMatrix Shrink(this BitMatrix m, Edge edge)
        {
            return edge switch
            {
                Edge.Left => RemoveColumnLeft(m),
                Edge.Right => RemoveColumnRight(m),
                Edge.Top => RemoveRowTop(m),
                _ => RemoveRowBottom(m)
            };
        }
        
        public static BitMatrix Grow(this BitMatrix m, Edge edge)
        {
            return edge switch
            {
                Edge.Left => AddColumnLeft(m),
                Edge.Right => AddColumnRight(m),
                Edge.Top => AddRowTop(m),
                _ => AddRowBottom(m)
            };
        }

        private static BitMatrix RemoveColumnLeft(BitMatrix m)
        {
            if (m.w <= 1) 
                return m;
            
            ulong outBits = 0;
            for (var y = 0; y < m.h; y++)
            {
                var row = (m.bits >> (y * m.w)) & RowMask(m.w);
                var shrunk = row >> 1; // Drop LSB
                outBits |= shrunk << (y * (m.w - 1));
            }
            
            return new BitMatrix((byte)(m.w - 1), m.h, outBits);
        }

        private static BitMatrix RemoveColumnRight(BitMatrix m)
        {
            if (m.w <= 1) 
                return m;
            
            ulong outBits = 0;
            var keep = RowMask(m.w - 1);
            for (var y = 0; y < m.h; y++)
            {
                var row = (m.bits >> (y * m.w)) & RowMask(m.w);
                var shrunk = row & keep; // Drop MSB
                outBits |= shrunk << (y * (m.w - 1));
            }
            
            return new BitMatrix((byte)(m.w - 1), m.h, outBits);
        }

        private static BitMatrix RemoveRowTop(BitMatrix m)
        {
            if (m.h <= 1)
                return m;
            
            ulong outBits = 0;
            for (var y = 1; y < m.h; y++)
            {
                var row = (m.bits >> (y * m.w)) & RowMask(m.w);
                outBits |= row << ((y - 1) * m.w);
            }
            
            return new BitMatrix(m.w, (byte)(m.h - 1), outBits);
        }

        private static BitMatrix RemoveRowBottom(BitMatrix m)
        {
            if (m.h <= 1) 
                return m;
          
            ulong outBits = 0;
            for (var y = 0; y < m.h - 1; y++)
            {
                var row = (m.bits >> (y * m.w)) & RowMask(m.w);
                outBits |= row << (y * m.w);
            }
            
            return new BitMatrix(m.w, (byte)(m.h - 1), outBits);
        }

        private static BitMatrix AddColumnLeft(BitMatrix m)
        {
            if ((long)(m.w + 1) * m.h > BitMatrix.MaxCells) 
                return m;

            ulong outBits = 0;
            for (var y = 0; y < m.h; y++)
            {
                var row = (m.bits >> (y * m.w)) & RowMask(m.w);
                var leftEdge = row & 1UL; // Old leftmost bit (LSB)
                var grown = (row << 1) | leftEdge; // Shift and copy edge into new col 0
                outBits |= grown << (y * (m.w + 1));
            }

            return new BitMatrix((byte)(m.w + 1), m.h, outBits);
        }

        private static BitMatrix AddColumnRight(BitMatrix m)
        {
            if ((long)(m.w + 1) * m.h > BitMatrix.MaxCells)
                return m;

            ulong outBits = 0;
            for (var y = 0; y < m.h; y++)
            {
                var row = (m.bits >> (y * m.w)) & RowMask(m.w);
                var rightEdge = (row >> (m.w - 1)) & 1UL; // Old rightmost bit (MSB of width w)
                var grown = row | (rightEdge << m.w); // Copy edge into new last column
                outBits |= grown << (y * (m.w + 1));
            }

            return new BitMatrix((byte)(m.w + 1), m.h, outBits);
        }


        private static BitMatrix AddRowTop(BitMatrix m)
        {
            if ((long)m.w * (m.h + 1) > BitMatrix.MaxCells)
                return m;
            
            ulong outBits = 0;
            // Copy old row top
            var topRow = m.bits & RowMask(m.w);
            outBits |= topRow << 0;
            
            // Shift existing rows down by 1
            for (var y = 0; y < m.h; y++)
            {
                var row = (m.bits >> (y * m.w)) & RowMask(m.w);
                outBits |= row << ((y + 1) * m.w);
            }
            
            return new BitMatrix(m.w, (byte)(m.h + 1), outBits);
        }

        private static BitMatrix AddRowBottom(BitMatrix m)
        {
            if ((long)m.w * (m.h + 1) > BitMatrix.MaxCells)
                return m;
            
            ulong outBits = 0;
            // Copy existing rows
            for (var y = 0; y < m.h; y++)
            {
                var row = (m.bits >> (y * m.w)) & RowMask(m.w);
                outBits |= row << (y * m.w);
            }
            
            // Copy old bottom row as the new one
            var bottomRow = (m.bits >> ((m.h - 1) * m.w)) & RowMask(m.w);
            outBits |= bottomRow << (m.h * m.w); 
            
            return new BitMatrix(m.w, (byte)(m.h + 1), outBits);
        }
        
        private static ulong RowMask(int w)
        {
            return w == 64 ? ulong.MaxValue : ((1UL << w) - 1UL);
        }
    }
}