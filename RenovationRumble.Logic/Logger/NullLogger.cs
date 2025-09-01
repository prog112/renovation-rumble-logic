namespace RenovationRumble.Logic.Logger
{
    /// <summary>
    /// No-op logger.
    /// </summary>
    public sealed class NullLogger : ILogicLogger
    {
        public static readonly NullLogger Logger = new NullLogger();
        
        public void Log(string message)
        {
            
        }

        public void LogError(string message)
        {
        }
    }
}