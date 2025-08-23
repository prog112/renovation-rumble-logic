namespace RenovationRumble.Logic.Data
{
    /// <summary>
    /// A lightweight engine-agnostic struct for board coordinates.
    /// </summary>
    public readonly struct Coords
    {
        public readonly byte x;
        public readonly byte y;

        public Coords(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Coords v1, Coords v2)
        {
            return v1.x == v2.x && v1.y == v2.y;
        }

        public static bool operator !=(Coords v1, Coords v2)
        {
            return v1.x != v2.x || v1.y != v2.y;
        }
        
        public override bool Equals(object? obj)
        {
            return obj is Coords c && this == c;
        }

        public override int GetHashCode()
        {
            return (x << 8) ^ y;
        }
    }
}