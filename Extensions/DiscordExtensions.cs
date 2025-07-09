using Disqord;
using Disqord.Bot.Commands.Interaction;
using Disqord.Gateway;

namespace MiataBot;

public static class DiscordExtensions
{
    public static LocalRowComponent InRow(this LocalComponent component)
        => LocalComponent.Row(component);
    
    public static DiscordInteractionResponseCommandResult AsEphemeral(this DiscordInteractionResponseCommandResult result, bool isEphemeral = true)
    {
        (result.Message as LocalInteractionMessageResponse)?.WithIsEphemeral(isEphemeral);
        return result;
    }
    
    public static string GetDisplayName(this IMember member)
        => member.Nick ?? member.GlobalName ?? member.Name;
    
    public static CachedRole? GetHighestRole(this IMember member, Func<CachedRole, bool>? func = null)
    {
        return member.GetRoles().Values
            .Where(func ?? (static _ => true))
            .OrderByDescending(x => x.Position)
            .ThenByDescending(x => x.Id)
            .FirstOrDefault();
    }
}