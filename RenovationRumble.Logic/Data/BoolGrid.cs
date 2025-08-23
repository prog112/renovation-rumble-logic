namespace RenovationRumble.Logic.Data
{
    /// <summary>
    /// A simple array is faster than multidimensional arrays in .NET.
    /// Exposing an indexer solves the awkward indexing giving us both speed and ease of use.
    /// </summary>
    public sealed class BoolGrid
    {
        public readonly int width;
        public readonly int height;
        
        private readonly bool[] grid;
        
        public BoolGrid(int width, int height)
        {
            grid = new bool[width * height];
        }

        public bool this[int x, int y]
        {
            get => grid[x + y * width];
            set => grid[x + y * width] = value;
        }
    }
}