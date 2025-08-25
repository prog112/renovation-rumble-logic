namespace RenovationRumble.Logic.Data
{
    using System;

    /// <summary>
    /// A lightweight engine-agnostic struct for board coordinates.
    /// </summary>
    [Serializable]
    public readonly struct Coords : IEquatable<Coords>
    {
        public readonly byte x;
        public readonly byte y;

        public Coords(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Coords c1, Coords c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coords c1, Coords c2)
        {
            return c1.x != c2.x || c1.y != c2.y;
        }
        
        public override bool Equals(object? obj)
        {
            return obj is Coords c && this == c;
        }

        public override int GetHashCode()
        {
            return (x << 8) ^ y;
        }

        public bool Equals(Coords other)
        {
            return x == other.x && y == other.y;
        }
    }
}