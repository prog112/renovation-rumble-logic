namespace RenovationRumble.Logic.Runtime.State
{
    using Board;
    using Wheel;

    public interface IReadOnlyGameState
    {
        IReadOnlyBoard Board { get; }
        IReadOnlyChoiceWheel Wheel { get; }
    }
    
    public sealed class GameState : IReadOnlyGameState
    {
        public Board Board { get; set; }
        public ChoiceWheel Wheel { get; set; }
        
        IReadOnlyBoard IReadOnlyGameState.Board => Board;
        IReadOnlyChoiceWheel IReadOnlyGameState.Wheel => Wheel;
    }
}