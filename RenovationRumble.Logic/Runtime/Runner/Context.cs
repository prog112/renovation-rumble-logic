namespace RenovationRumble.Logic.Runtime.Runner
{
    using Board;
    using Catalog;
    using Logger;

    public sealed class Context
    {
        public ILogicLogger Logger { get; set; }
        public GameData GameData { get; set; }
        
        public Board Board { get; set; }
    }
}