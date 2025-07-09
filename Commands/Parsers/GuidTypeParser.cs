using Disqord.Bot.Commands;
using Qmmands;

namespace MiataBot;

public sealed class GuidTypeParser : DiscordTypeParser<Guid>
{
    public override ValueTask<ITypeParserResult<Guid>> ParseAsync(IDiscordCommandContext context, IParameter parameter, ReadOnlyMemory<char> value)
    {
        return !Guid.TryParse(value.Span, out var guid)
            ? Failure("The supplied ID was improperly formatted.")
            : Success(guid);
    }
}