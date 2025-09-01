namespace RenovationRumble.Logic.Runtime.Runner
{
    using System;
    using System.Collections.Generic;
    using Data.Commands;
    using Executors;

    public sealed class CommandRunner
    {
        private interface IAdapter
        {
            bool CanApply(in Context context, CommandDataModel command);
            void Apply(Context context, CommandDataModel command);
        }

        private sealed class Adapter<T> : IAdapter where T : CommandDataModel
        {
            private readonly ICommandExecutor<T> executor;

            public Adapter(ICommandExecutor<T> executor)
            {
                this.executor = executor;
            }
                
            public bool CanApply(in Context context, CommandDataModel command)
            {
                return executor.CanApply(in context, (T)command);
            }

            public void Apply(Context context, CommandDataModel command)
            {
                executor.Apply(context, (T)command);
            }
        }

        private readonly Dictionary<Type, IAdapter> executors = new Dictionary<Type, IAdapter>();
        
        public void Register<T>(ICommandExecutor<T> executor) where T : CommandDataModel
        {
            executors.TryAdd(typeof(T), new Adapter<T>(executor));
        }
        
        public CommandResult TryApplyCommand(in Context context, CommandDataModel command)
        {
            if(context == null)
                return CommandResult.Fail(CommandError.InvalidCommand, "game match not started");
            
            if(command == null)
                return CommandResult.Fail(CommandError.InvalidCommand, "command is null");
            
            if (!executors.TryGetValue(command.GetType(), out var executor))
                return CommandResult.Fail(CommandError.InvalidCommand, "no executor found");

            bool canApply;
            try
            {
                canApply = executor.CanApply(in context, command);
            }
            catch (Exception e)
            {
                return CommandResult.Fail(CommandError.ValidationException, e.Message);
            }
            
            if(!canApply)
                return CommandResult.Fail(CommandError.ValidationFailed);

            try
            {
                executor.Apply(context, command);
            }
            catch (Exception e)
            {
                return CommandResult.Fail(CommandError.ApplyException, e.Message);
            }
            
            return CommandResult.Ok();
        }
    }
}