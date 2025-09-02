namespace RenovationRumble.Logic.Runtime.Runner
{
    using Catalog;
    using Logger;
    using State;

    /// <summary>
    /// This is a readonly struct to avoid accidental mutation when firing CanApply for commands.
    /// Also, this allows us to pass around Context everywhere else and create a cheap readonly instance only when needed.
    /// </summary>
    public readonly struct ReadOnlyContext
    {
        public ILogicLogger Logger { get; }
        public GameData Data { get; }
        public IReadOnlyGameState State { get; }
        
        public ReadOnlyContext(in Context context)
        {
            Logger = context.Logger;
            Data = context.Data;
            State = context.State;
        }
    }
    
    public sealed class Context
    {
        public ILogicLogger Logger { get; set; }
        public GameData Data { get; set; }
        public GameState State { get; set; }
    }
}