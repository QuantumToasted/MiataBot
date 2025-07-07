using Disqord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiataBot;

// Allows for more extensibility beyond just LFM
public sealed record TimedRoleEntry : IEntityTypeConfiguration<TimedRoleEntry>
{
    public int Id { get; init; }
    
    public Snowflake GuildId { get; init; }
    
    public Snowflake UserId { get; init; }

    // These roles will be revoked when expiring — "what roles were granted when this entry was created?"
    public Snowflake[] GrantedRoleIds { get; init; } = [];

    // These roles will be granted when expiring — "what roles were revoked when this entry was created?"
    public Snowflake[] RevokedRoleIds { get; init; } = [];
    
    public DateTimeOffset ExpiresAt { get; init; }
    
    // relations
    
    public GuildConfiguration? Guild { get; init; }
    
    void IEntityTypeConfiguration<TimedRoleEntry>.Configure(EntityTypeBuilder<TimedRoleEntry> entry)
    {
        entry.HasKey(x => x.Id);

        entry.HasIndex(x => x.GuildId).IsUnique(false);
        
        entry.HasIndex(x => x.UserId).IsUnique(false);
    }
}