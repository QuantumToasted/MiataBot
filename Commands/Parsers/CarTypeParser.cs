using Disqord.Bot.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace MiataBot;

public sealed class CarTypeParser : DiscordTypeParser<Car>
{
    public override async ValueTask<ITypeParserResult<Car>> ParseAsync(IDiscordCommandContext context, IParameter parameter, ReadOnlyMemory<char> value)
    {
        const string failureMessage =
            "The supplied car ID does not exist — you MIGHT need to report this to a developer if you know what you're doing.";
        
        if (!Guid.TryParse(value.Span, out var carId))
            return Failure(failureMessage);
        
        // This db context is disposed of later
        var db = context.Services.GetRequiredService<MiataDbContext>();
        if (await db.Cars.FirstOrDefaultAsync(x => x.Id == carId) is not { } car)
            return Failure(failureMessage);

        return Success(car);
    }
}