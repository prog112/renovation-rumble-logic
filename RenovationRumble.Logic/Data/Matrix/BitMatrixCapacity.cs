namespace RenovationRumble.Logic.Data.Matrix
{
    /// <summary>
    /// Helper for reasoning about how far a <see cref="BitMatrix"/> can grow along width/height
    /// given the fixed backing capacity (<see cref="BitMatrix.MaxCells"/>).
    /// All formulas assume typical row-major packing.
    /// </summary>
    public static class BitMatrixCapacity
    {
        public static int MaxWidthForHeight(int h)
        {
            return h <= 0 ? 0 : BitMatrix.MaxCells / h;
        }

        public static int MaxHeightForWidth(int w)
        {
            return w <= 0 ? 0 : BitMatrix.MaxCells / w;
        }

        public static int ColumnsRemaining(int w, int h)
        {
            var maxW = MaxWidthForHeight(h);
            var left = maxW - w;
            return left < 0 ? 0 : left;
        }

        public static int RowsRemaining(int w, int h)
        {
            var maxH = MaxHeightForWidth(w);
            var left = maxH - h;
            return left < 0 ? 0 : left;
        }

        public static bool CanGrowColumn(int w, int h)
        {
            return (long)(w + 1) * h <= BitMatrix.MaxCells;
        }

        public static bool CanGrowRow(int w, int h)
        {
            return (long)w * (h + 1) <= BitMatrix.MaxCells;
        }
    }
}