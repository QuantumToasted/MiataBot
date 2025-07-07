using Disqord.Bot.Commands;
using Qmmands;
using Qommon;

namespace MiataBot;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class RequireCarOwnerAttribute : DiscordParameterCheckAttribute
{
    public override bool CanCheck(IParameter parameter, object? value) => value is Car;

    public override ValueTask<IResult> CheckAsync(IDiscordCommandContext context, IParameter parameter, object? argument)
    {
        if (argument is not Car car)
            throw new InvalidOperationException();

        return car.OwnerId != context.AuthorId
            ? Results.Failure("You do not own this car.")
            : Results.Success;
    }
}