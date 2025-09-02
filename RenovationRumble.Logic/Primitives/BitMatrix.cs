namespace RenovationRumble.Logic.Primitives
{
    using System;

    /// <summary>
    /// A stack-allocated rectangular matrix supporting values of 1 and 0,
    /// stored compactly as a 64-bit bitfield in row-major order.
    /// This type avoids the overhead of bool arrays, giving us zero heap allocations.
    /// </summary>
    [Serializable]
    public readonly struct BitMatrix
    {
        public struct BitMatrixEnumerator
        {
            private readonly byte w;
            private ulong bitsLeft;
            private (byte x, byte y) current;

            internal BitMatrixEnumerator(BitMatrix matrix)
            {
                w = matrix.w;
                bitsLeft = matrix.bits;
                current = default;
            }

            public (byte x, byte y) Current => current;
            public BitMatrixEnumerator GetEnumerator() => this;

            public bool MoveNext()
            {
                if (bitsLeft == 0UL) 
                    return false;

                // By looking at where the next 1 is, we can easily establish the 1d index
                var currentFlatIndex = BitOperations.TrailingZeroCount(bitsLeft);
                bitsLeft &= bitsLeft - 1; // Clear the lowest set bit

                // Convert to 2d indices
                var y = (byte)(currentFlatIndex / w);
                var x = (byte)(currentFlatIndex - y * w);
                current = (x, y);
                return true;
            }
        }

        public readonly byte w;
        public readonly byte h;
        public readonly ulong bits;
        
        public const int MaxCells = 64; // ulong is 64 bits

        public BitMatrix(byte w, byte h, ulong bits)
        {
            if (w <= 0) 
                throw new ArgumentOutOfRangeException(nameof(w));
            
            if (h <= 0)
                throw new ArgumentOutOfRangeException(nameof(h));
            
            if ((long)w * h > MaxCells) 
                throw new ArgumentOutOfRangeException(null, $"BitMatrix supports up to {MaxCells} cells. Got: {w}×{h} = {(long)w * h}.");
           
            this.w = w;
            this.h = h;
            this.bits = bits;
        }
        
        public BitMatrixEnumerator FilledCells()
        {
            // Use a custom enumerator to avoid heap allocs
            return new BitMatrixEnumerator(this);
        }

        public bool this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= w)
                    throw new ArgumentOutOfRangeException(nameof(x));

                if (y < 0 || y >= h)
                    throw new ArgumentOutOfRangeException(nameof(y));

                return ((bits >> (y * w + x)) & 1UL) != 0;
            }
        }

        public BitMatrix Rotate90()
        {
            ulong outBits = 0;
            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    if (this[x, y])
                    {
                        var nx = h - 1 - y;
                        var ny = x;
                        outBits |= 1UL << (ny * h + nx);
                    }
                }
            }

            return new BitMatrix(h, w, outBits);
        }

        public BitMatrix Rotate180()
        {
            ulong outBits = 0;
            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    if (this[x, y])
                    {
                        var nx = w - 1 - x;
                        var ny = h - 1 - y;
                        outBits |= 1UL << (ny * w + nx);
                    }
                }
            }

            return new BitMatrix(w, h, outBits);
        }

        public BitMatrix Rotate270()
        {
            ulong outBits = 0;
            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    if (this[x, y])
                    {
                        var nx = y;
                        var ny = w - 1 - x;
                        outBits |= 1UL << (ny * h + nx);
                    }
                }
            }

            return new BitMatrix(h, w, outBits);
        }
    }
}