namespace RenovationRumble.Logic.Utility
{
    /// <summary>
    /// Uses the De Bruijn algorithm to quickly bit scan in O(1) time.
    /// See: https://stackoverflow.com/questions/21888140/de-bruijn-algorithm-binary-digit-count-64bits-c-sharp 
    /// </summary>
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

        public static int TrailingZeroCount(ulong v)
        {
            if (v == 0) 
                return -1;
            
            return _Index64[((ulong)((long)v & -(long)v) * Multiplicator) >> 58];
        }
    }
}