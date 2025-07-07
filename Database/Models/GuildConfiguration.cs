using Disqord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiataBot;

public sealed record GuildConfiguration : IEntityTypeConfiguration<GuildConfiguration>
{
    public Snowflake Id { get; init; }
    
    // relations
    
    public List<CopypastaEntry>? Copypastas { get; init; }
    
    public List<TimedRoleEntry>? TimedRoleEntries { get; init; }
    
    void IEntityTypeConfiguration<GuildConfiguration>.Configure(EntityTypeBuilder<GuildConfiguration> config)
    {
        config.HasKey(x => x.Id);

        config.HasMany(x => x.Copypastas).WithOne(x => x.Guild).HasForeignKey(x => x.GuildId);
    }
}