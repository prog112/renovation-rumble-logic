namespace RenovationRumble.Logic.Logger
{
    /// <summary>
    /// A simple logger facade for the logic layer. Clients can plug in their own implementation
    /// without adding dependencies to the core.
    /// </summary>
    public interface ILogicLogger
    {
        void Log(string message);
        void LogError(string message);
    }
}