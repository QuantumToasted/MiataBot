using Disqord.Bot.Hosting;
using Microsoft.EntityFrameworkCore;

namespace MiataBot;

public sealed class QuartzService(IDbContextFactory<MiataDbContext> dbFactory) : DiscordBotService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Bot.WaitUntilReadyAsync(stoppingToken);

        await using var db = await dbFactory.CreateDbContextAsync(stoppingToken);

        var timedRoleEntries = await db.TimedRoleEntries.ToListAsync(stoppingToken);
    }
}