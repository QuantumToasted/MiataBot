using Disqord.Bot.Commands.Components;
using Microsoft.EntityFrameworkCore;
using Qmmands;

namespace MiataBot;

public sealed class CarComponentModule(MiataDbContext db) : DiscordComponentGuildModuleBase
{
    private static readonly Dictionary<string, int> Months = new()
    {
        ["january"] = 1,
        ["jan"] = 1,
        ["1"] = 1,
        ["february"] = 2,
        ["feb"] = 2,
        ["2"] = 2,
        ["march"] = 3,
        ["mar"] = 3,
        ["3"] = 3,
        ["april"] = 4,
        ["apr"] = 4,
        ["4"] = 4,
        ["may"] = 5,
        ["5"] = 5,
        ["june"] = 6,
        ["jun"] = 6,
        ["6"] = 6,
        ["july"] = 7,
        ["jul"] = 7,
        ["7"] = 7,
        ["august"] = 8,
        ["aug"] = 8,
        ["8"] = 8,
        ["september"] = 9,
        ["sep"] = 9,
        ["9"] = 9,
        ["october"] = 10,
        ["oct"] = 10,
        ["10"] = 10,
        ["november"] = 11,
        ["nov"] = 11,
        ["11"] = 11,
        ["december"] = 12,
        ["dec"] = 12,
        ["12"] = 12
    };
    
    [ModalCommand("CarInfo:Update:OwnedSince:*")]
    public async Task<IResult> UpdateOwnedSince(
        Guid carId,
        string? year = null,
        string? month = null,
        string? day = null)
    {
        var currentYear = DateTime.Now.Year;
        if (!int.TryParse(year, out var actualYear) ||
            actualYear < 1900 || actualYear > currentYear + 1 ||
            !Months.TryGetValue(month!.ToLower(), out var actualMonth) ||
            !int.TryParse(day, out var actualDay) ||
            !DateOnly.TryParse($"{actualMonth}/{actualDay}/{actualYear}", out var ownedSince))
        {
            return Response("The supplied date was not properly formatted.").AsEphemeral();
        }

        await db.Cars.Where(x => x.Id == carId).ExecuteUpdateAsync(x => x.SetProperty(y => y.OwnedSince, ownedSince));
        return Response("Car purchase or acquisition date updated!").AsEphemeral();
    }

    [ModalCommand("CarInfo:Update:Color:*")]
    public async Task<IResult> UpdateColor(
        Guid carId,
        string? color = null)
    {
        if (string.IsNullOrWhiteSpace(color))
            return Response("You must supply an updated color.");
        
        await db.Cars.Where(x => x.Id == carId).ExecuteUpdateAsync(x => x.SetProperty(y => y.Color, color));
        return Response("Car color updated!").AsEphemeral();
    }
    
    [ModalCommand("CarInfo:Update:PetName:*")]
    public async Task<IResult> UpdatePetName(
        Guid carId,
        string? name = null)
    {
        await db.Cars.Where(x => x.Id == carId).ExecuteUpdateAsync(x => x.SetProperty(y => y.PetName, name));
        return Response(string.IsNullOrWhiteSpace(name)
            ? "Car nickname removed!"
            : "Car nickname updated!").AsEphemeral();
    }
    
    [ModalCommand("CarInfo:Update:Blurb:*")]
    public async Task<IResult> UpdateBlurb(
        Guid carId,
        string? blurb = null)
    {
        await db.Cars.Where(x => x.Id == carId).ExecuteUpdateAsync(x => x.SetProperty(y => y.Blurb, blurb));
        
        return Response(string.IsNullOrWhiteSpace(blurb)
            ? "Car blurb removed!"
            : "Car blurb updated!").AsEphemeral();
    }
    
    [ModalCommand("CarInfo:Update:VIN:*")]
    public async Task<IResult> UpdateVIN(
        Guid carId,
        string? vin = null)
    {
        await db.Cars.Where(x => x.Id == carId).ExecuteUpdateAsync(x => x.SetProperty(y => y.VIN, vin));
        return Response("Car VIN updated!").AsEphemeral();
    }
}