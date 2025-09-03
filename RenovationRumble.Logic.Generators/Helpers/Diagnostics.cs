namespace RenovationRumble.Logic.Generators.Helpers
{
    using Microsoft.CodeAnalysis;

    public static class Diagnostics
    {
        public static readonly DiagnosticDescriptor InvalidAttributeUsage = new(
            id: "RR_01",
            title: "Invalid JsonDiscriminatedUnion usage",
            messageFormat: "Type '{0}' has invalid JsonDiscriminatedUnion arguments.",
            category: "Generation",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor MissingOverride = new(
            id: "RR_02",
            title: "Subclass must override enum discriminator property",
            messageFormat: "Type '{0}' must override the enum property specified on the base.",
            category: "Generation",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor UnsupportedGetter = new(
            id: "RR_03",
            title: "Unsupported enum property getter",
            messageFormat: "Type '{0}' has an enum getter the generator cannot evaluate. Use '=> Enum.Member'.",
            category: "Generation",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor DuplicateEnumValue = new(
            id: "RR_04",
            title: "Duplicate enum discriminator value",
            messageFormat: "Enum member '{0}' is assigned to multiple subclasses: {1}",
            category: "Generation",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor MissingPublicParameterlessCtor = new(
            id: "RR_05",
            title: "Executor must have a public parameterless constructor",
            messageFormat: "Executor type '{0}' must define a public parameterless constructor to be auto-registered.",
            category: "Generation",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor MultipleExecutorInterfaces = new(
            id: "RR_06",
            title: "Executor implements multiple ICommandExecutor<T> interfaces",
            messageFormat: "Executor type '{0}' implements multiple ICommandExecutor<T> interfaces. Use one executor per command type.",
            category: "Generation",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor DuplicateExecutorForCommand = new(
            id: "RR_07",
            title: "Duplicate executors for the same command type",
            messageFormat: "Command type '{0}' has multiple executors: {1}",
            category: "Generation",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}