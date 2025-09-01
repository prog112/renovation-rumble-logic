namespace RenovationRumble.Logic.Runtime.Runner
{
    using Board;
    using Logger;

    public sealed class Context
    {
        public ILogicLogger Logger { get; set; }
        public RotationCache RotationCache { get; set; }
        
        public Board Board { get; set; }
    }
}