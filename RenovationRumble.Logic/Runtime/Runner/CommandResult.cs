namespace RenovationRumble.Logic.Runtime.Runner
{
    public enum CommandError : byte
    {
        None = 0,
        InvalidCommand,
        ValidationFailed,
        ValidationException,
        ApplyException
    }
    
    public readonly struct CommandResult
    {
        public readonly bool isSuccess;
        public readonly CommandError error;
        public readonly string message;
        
        private CommandResult(bool isSuccess, CommandError error, string message)
        {
            this.isSuccess = isSuccess;
            this.error = error;
            this.message = message;
        }

        public static CommandResult Ok() => new CommandResult(true, CommandError.None, null);
        public static CommandResult Fail(CommandError error, string message = null) => new CommandResult(false, error, message);
    }
}