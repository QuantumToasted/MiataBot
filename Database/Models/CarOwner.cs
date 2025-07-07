using Disqord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiataBot;

public sealed record CarOwner : IEntityTypeConfiguration<CarOwner>
{
    public Snowflake UserId { get; init; }
    
    public Guid? DefaultCarId { get; set; }

    // relations

    public List<Car>? Cars { get; init; }
    
    void IEntityTypeConfiguration<CarOwner>.Configure(EntityTypeBuilder<CarOwner> owner)
    {
        owner.HasKey(x => x.UserId);
        
        owner.HasMany(x => x.Cars).WithOne(x => x.Owner).HasForeignKey(x => x.OwnerId);
    }
}