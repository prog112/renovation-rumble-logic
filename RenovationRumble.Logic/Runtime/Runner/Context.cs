namespace RenovationRumble.Logic.Runtime.Runner
{
    using Catalog;
    using Logger;
    using State;

    public sealed class Context
    {
        public ILogicLogger Logger { get; set; }
        public GameData Data { get; set; }
        public GameState State { get; set; }
    }
}