namespace RenovationRumble.Logic.Generators.Helpers
{
    public static class ProjectMetadata
    {
        // Command execution
        public const string ExecutorInterface = "RenovationRumble.Logic.Runtime.Executors.ICommandExecutor`1";
        public const string CommandBase = "RenovationRumble.Logic.Data.Commands.CommandDataModel";
        public const string CommandRunner = "RenovationRumble.Logic.Runtime.Runner.CommandRunner";

        // Discriminated union
        public const string DiscriminatedUnionAttribute = "RenovationRumble.Logic.Serialization.DiscriminatedUnionAttribute";
    }
}