using Disqord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiataBot;

public sealed record CopypastaEntry : IEntityTypeConfiguration<CopypastaEntry>
{
    public int Id { get; init; }
    
    public Snowflake GuildId { get; init; }

    public Snowflake UserId { get; init; }

    public string Text { get; init; } = null!;
    
    // relations
    
    public GuildConfiguration? Guild { get; init; }

    void IEntityTypeConfiguration<CopypastaEntry>.Configure(EntityTypeBuilder<CopypastaEntry> entry)
    {
        entry.HasKey(x => x.Id);
        
        entry.HasIndex(x => x.GuildId).IsUnique(false);

        entry.HasIndex(x => x.UserId).IsUnique(false);
    }
}