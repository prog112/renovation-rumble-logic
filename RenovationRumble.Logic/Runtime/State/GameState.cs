namespace RenovationRumble.Logic.Runtime.State
{
    using Board;

    public interface IReadOnlyGameState
    {
        IReadOnlyBoard Board { get; }
    }
    
    public sealed class GameState : IReadOnlyGameState
    {
        public Board Board { get; set; }
        IReadOnlyBoard IReadOnlyGameState.Board => Board;
    }
}