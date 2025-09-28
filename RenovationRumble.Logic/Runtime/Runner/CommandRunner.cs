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
            bool CanApply(in ReadOnlyContext context, CommandDataModel command);
            void Apply(Context context, CommandDataModel command);
        }

        private sealed class Adapter<T> : IAdapter where T : CommandDataModel
        {
            private readonly ICommandExecutor<T> executor;

            public Adapter(ICommandExecutor<T> executor)
            {
                this.executor = executor;
            }
                
            public bool CanApply(in ReadOnlyContext context, CommandDataModel command)
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
            if (executor is null)
                throw new ArgumentNullException(nameof(executor));

            if (!executors.TryAdd(typeof(T), new Adapter<T>(executor)))
                throw new InvalidOperationException($"Executor for {typeof(T).Name} already registered.");
        }

        public CommandResult TryApplyCommand(in Context context, CommandDataModel command)
        {
            if(context == null || context.State == null)
                return CommandResult.Fail(CommandError.InvalidCommand, "Invalid context/game state!");
            
            if(command == null)
                return CommandResult.Fail(CommandError.InvalidCommand, "Command is null!");
            
            if (!TryResolveAdapter(command.GetType(), out var adapter))
                return CommandResult.Fail(CommandError.InvalidCommand, "No compatible executor found!");

            bool canApply;
            try
            {
                canApply = adapter.CanApply(new ReadOnlyContext(context), command);
            }
            catch (Exception e)
            {
                context.Logger.LogError($"Validation exception during command {command.GetType().Name}: {e}.");
                return CommandResult.Fail(CommandError.ValidationException, e.Message);
            }
            
            if(!canApply)
                return CommandResult.Fail(CommandError.ValidationFailed);

            try
            {
                adapter.Apply(context, command);
            }
            catch (Exception e)
            {
                context.Logger.LogError($"Execution exception during command {command.GetType().Name}: {e}.");
                return CommandResult.Fail(CommandError.ApplyException, e.Message);
            }
            
            return CommandResult.Ok();
        }
        
        private bool TryResolveAdapter(Type commandType, out IAdapter adapter)
        {
            // Check the cache first
            if (executors.TryGetValue(commandType, out adapter))
                return true;

            // Walk base types to find the nearest parent
            var type = commandType.BaseType;
            while (type != null && typeof(CommandDataModel).IsAssignableFrom(type))
            {
                if (executors.TryGetValue(type, out adapter))
                {
                    // Save into our cache 
                    executors[commandType] = adapter;
                    return true;
                }
                type = type.BaseType;
            }

            return false;
        }
    }
}