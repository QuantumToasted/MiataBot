using Disqord;
using Disqord.Bot;
using Disqord.Bot.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qmmands;
using Qmmands.Default;

namespace MiataBot;

public sealed class MiataDiscordBot(
    IOptions<DiscordBotConfiguration> options,
    ILogger<DiscordBot> logger,
    IServiceProvider services,
    DiscordClient client)
    : DiscordBot(options, logger, services, client)
{
    protected override ValueTask AddTypeParsers(DefaultTypeParserProvider typeParserProvider, CancellationToken cancellationToken)
    {
        typeParserProvider.AddParser(new CarTypeParser());
        return base.AddTypeParsers(typeParserProvider, cancellationToken);
    }
    
    protected override string? FormatFailureReason(IDiscordCommandContext context, IResult result)
    {
        if (result is ExceptionResult exceptionResult)
        {
            return "An unhandled exception has occurred. please report this to a developer:\n" +
                   Markdown.CodeBlock($"{exceptionResult.Exception.GetType()}: {exceptionResult.Exception.Message}");
        }

        return base.FormatFailureReason(context, result);
    }
}