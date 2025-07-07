using Disqord.Bot.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MiataBot;

public sealed partial class DatabaseMigrationService(IDbContextFactory<MiataDbContext> dbFactory, ILogger<DatabaseMigrationService> logger) : DiscordBotService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);

        var migrations = (await db.Database.GetPendingMigrationsAsync(cancellationToken)).ToArray();
        
        if (migrations.Length > 0)
        {
            MigrationsApplied(migrations.Length, migrations);
            await db.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            NoMigrations();
        }
    }
    
    [LoggerMessage(Message = "Applying {Count} migration(s): {Migrations}", Level = LogLevel.Information)]
    private partial void MigrationsApplied(int count, string[] migrations);
    
    [LoggerMessage(Message = "No pending migrations!", Level = LogLevel.Information)]
    private partial void NoMigrations();
}