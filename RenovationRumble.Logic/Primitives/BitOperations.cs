namespace RenovationRumble.Logic.Primitives
{
    public static class BitOperations
    {
        private static readonly int[] _Index64 =
        {
            0, 1, 2,53, 3, 7,54,27,
            4,38,41, 8,34,55,48,28,
            62, 5,39,46,44,42,22, 9,
            24,35,59,56,49,18,29,11,
            63,52, 6,26,37,40,33,47,
            61,45,43,21,23,58,17,10,
            51,25,36,32,60,20,57,16,
            50,31,19,15,30,14,13,12
        };

        private const ulong Multiplicator = 0x022fdd63cc95386dUL;

        /// <summary>
        /// Returns the number of consecutive zero bits starting from the least significant bit (LSB).
        /// Uses the De Bruijn algorithm to quickly bit scan in O(1) time.
        /// See: https://stackoverflow.com/questions/21888140/de-bruijn-algorithm-binary-digit-count-64bits-c-sharp 
        /// </summary>
        public static int TrailingZeroCount(ulong v)
        {
            // Zero has all 64 bits set as 0
            if (v == 0) 
                return 64;
            
            return _Index64[((ulong)((long)v & -(long)v) * Multiplicator) >> 58];
        }
        
        /// <summary>
        /// Counts the number of set bits (1s) in the given 64-bit unsigned integer.
        /// Uses the SWAR (SIMD Within A Register) bit counting algorithm.
        /// See: https://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
        /// </summary>
        public static int PopCount(ulong v)
        {
            v = v - ((v >> 1) & 0x5555_5555_5555_5555UL);
            v = (v & 0x3333_3333_3333_3333UL) + ((v >> 2) & 0x3333_3333_3333_3333UL);
            v = (v + (v >> 4)) & 0x0F0F_0F0F_0F0F_0F0FUL;
            v = v + (v >> 8);
            v = v + (v >> 16);
            v = v + (v >> 32);
            return (int)(v & 0x7F);
        }
    }
}