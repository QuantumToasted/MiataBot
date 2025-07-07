using Qmmands;

namespace MiataBot;

public sealed class CarAutoCompleteFormatter : IAutoCompleteFormatter<Car, string>
{
    public static string FormatAutoCompleteName(ICommandContext context, Car model) => $"{model.Year} {model.Make} {model.Model} | {model.Color}";

    public static string FormatAutoCompleteValue(ICommandContext context, Car model) => model.Id.ToString();

    public static string[] FormatComparisonValues(ICommandContext context, Car model) => [model.Make, model.Model, model.Color, model.Year.ToString()];
}