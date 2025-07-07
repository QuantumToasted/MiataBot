using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiataBot;

public sealed record CarMedia : IEntityTypeConfiguration<CarMedia>
{
    public Guid ObjectKey { get; init; } = Guid.NewGuid();

    public Guid CarId { get; init; }
    
    // relations
    
    public Car? Car { get; init; }

    void IEntityTypeConfiguration<CarMedia>.Configure(EntityTypeBuilder<CarMedia> attachment)
    {
        attachment.HasKey(x => x.ObjectKey);
    }
}