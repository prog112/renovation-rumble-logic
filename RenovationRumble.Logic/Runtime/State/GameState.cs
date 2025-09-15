namespace RenovationRumble.Logic.Runtime.State
{
    using Board;
    using Runner;
    using Wheel;

    public interface IReadOnlyGameState
    {
        IReadOnlyBoard Board { get; }
        IReadOnlyChoiceWheel Wheel { get; }
        GamePhase Phase { get; }
        uint Moves { get; }
    }
    
    public sealed class GameState : IReadOnlyGameState
    {
        public Board Board { get; set; }
        public ChoiceWheel Wheel { get; set; }
        public GamePhase Phase { get; set; }
        public uint Moves { get; set; }
        
        IReadOnlyBoard IReadOnlyGameState.Board => Board;
        IReadOnlyChoiceWheel IReadOnlyGameState.Wheel => Wheel;
    }
}