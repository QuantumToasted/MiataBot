using Disqord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiataBot;

public sealed record Car : IEntityTypeConfiguration<Car>
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public Snowflake OwnerId { get; init; }
    
    public DateOnly OwnedSince { get; init; }
    
    public int Year { get; init; }

    public string Make { get; init; } = null!;
    
    public string Model { get; init; } = null!;
    
    public string Color { get; init; } = null!;
    
    public string? PetName { get; init; }

    public string? Blurb { get; init; }
    
    public string? VIN { get; init; }
    
    // relations
    
    public CarOwner? Owner { get; init; }

    public List<CarMedia>? Media { get; init; }

    public List<CarMetadata>? Metadata { get; init; }

    void IEntityTypeConfiguration<Car>.Configure(EntityTypeBuilder<Car> car)
    {
        car.HasKey(x => x.Id);

        car.HasIndex(x => x.OwnerId).IsUnique(false);

        car.HasMany(x => x.Media).WithOne(x => x.Car).HasForeignKey(x => x.CarId);

        car.HasMany(x => x.Metadata).WithOne(x => x.Car).HasForeignKey(x => x.CarId);
    }
}