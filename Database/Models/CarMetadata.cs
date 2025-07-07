using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiataBot;

public sealed record CarMetadata : IEntityTypeConfiguration<CarMetadata>
{
    public string Name { get; init; } = null!;

    public string Value { get; set; } = null!;
    
    public Guid CarId { get; init; }
    
    // relations
    
    public Car? Car { get; init; }
    
    void IEntityTypeConfiguration<CarMetadata>.Configure(EntityTypeBuilder<CarMetadata> metadata)
    {
        metadata.HasKey(x => new { x.CarId, Key = x.Name });
    }
}