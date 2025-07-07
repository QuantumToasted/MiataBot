using Disqord;
using Microsoft.EntityFrameworkCore;

namespace MiataBot;

public sealed class MiataDbContext(DbContextOptions<MiataDbContext> options) : DbContext(options)
{
    public DbSet<Car> Cars { get; init; }
    
    public DbSet<CarMedia> Media { get; init; }
    
    public DbSet<CarMetadata> Metadata { get; init; }
    
    public DbSet<CarOwner> Owners { get; init; }
    
    public DbSet<CopypastaEntry> Copypastas { get; init; }
    
    public DbSet<GuildConfiguration> GuildConfigurations { get; init; }
    
    public DbSet<TimedRoleEntry> TimedRoleEntries { get; init; }
    
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
    {
        configuration.Properties<Snowflake>().HaveConversion<SnowflakeConverter>();
        configuration.Properties<Snowflake[]>().HaveConversion<SnowflakeArrayConverter, ArrayValueComparer<Snowflake>>();
    }
}