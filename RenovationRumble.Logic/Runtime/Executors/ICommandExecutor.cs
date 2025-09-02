namespace RenovationRumble.Logic.Runtime.Executors
{
    using Data.Commands;
    using Runner;

    /// <summary>
    /// Contract interface for command executors.
    /// </summary>
    public interface ICommandExecutor<in TCommand> where TCommand : CommandDataModel
    {
        bool CanApply(in ReadOnlyContext context, TCommand command);
        void Apply(Context context, TCommand command);
    }
}