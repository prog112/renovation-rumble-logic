namespace RenovationRumble.Logic.Data.Commands
{
    public enum Command : byte
    {
        Place,
        Grow,
        Shrink,
    }
    
    public abstract class CommandDataModel
    {
        /// <summary>
        /// Must be provided by derived classes to safely resolve the command type in serialization.
        /// </summary>
        public abstract Command Command { get; }
    }
}