namespace RenovationRumble.Logic.Primitives
{
    public enum Edge : byte
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public static class EdgeExtensions
    {
        public static bool IsHorizontal(this Edge edge)
        {
            return edge == Edge.Left || edge == Edge.Right;
        }
        
        public static bool IsVertical(this Edge edge)
        {
            return edge == Edge.Top || edge == Edge.Bottom;
        }
    }
}