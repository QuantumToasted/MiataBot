using Qmmands;

namespace MiataBot;

public sealed class StringAutoCompleteFormatter : IAutoCompleteFormatter<string, string>
{
    public static string FormatAutoCompleteName(ICommandContext context, string model) => model;
    public static string FormatAutoCompleteValue(ICommandContext context, string model) => model;
    public static string[] FormatComparisonValues(ICommandContext context, string model) => [model];
}