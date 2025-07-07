using Qmmands;

namespace MiataBot;

public sealed class CarMetadataAutoCompleteFormatter : IAutoCompleteFormatter<CarMetadata, string>
{
    public static string FormatAutoCompleteName(ICommandContext context, CarMetadata model) => model.Format();

    public static string FormatAutoCompleteValue(ICommandContext context, CarMetadata model) => model.Name;

    public static string[] FormatComparisonValues(ICommandContext context, CarMetadata model) => [model.Name, model.Value];
}