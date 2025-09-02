namespace RenovationRumble.Logic.Data.Commands
{
    using Logic.Serialization;

    public enum Command : byte
    {
        Place,
        Grow,
        Shrink,
    }
    
    [DiscriminatedUnion(typeof(Command), nameof(Command))]
    public abstract class CommandDataModel
    {
        /// <summary>
        /// Must be provided by derived classes to safely resolve the command type in serialization.
        /// </summary>
        public abstract Command Command { get; }
    }
}