using Amazon.S3;
using Amazon.S3.Model;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Commands.Application;
using Microsoft.EntityFrameworkCore;
using Qmmands;
using Qommon;

namespace MiataBot;

[SlashGroup("car")]
public sealed class CarModule(MiataDbContext db, AttachmentService attachments) : DiscordApplicationGuildModuleBase
{
    [MutateModule]
    public static void MutateModule(DiscordBotBase bot, IModuleBuilder builder)
    {
        var addCommand = builder.Commands.OfType<ApplicationCommandBuilder>().First(x => x.Alias == "add");
        var yearParameter = addCommand.Parameters.First(x => x.Name == "year");
        yearParameter.Checks.Add(new RangeAttribute(1900, DateTimeOffset.UtcNow.Year + 1)); // "current" model year may be next year
    }

    [SlashCommand("info")]
    [Description("Views information about a user's car.")]
    public async Task<IResult> Info(
        [Name("user")]
        [Description("The user to view. Defaults to yourself.")]
            IMember? member = null,
        [Description("Choose the user's car to view, or their \"default\" car if not provided.")]
            Car? car = null)
    {
        member ??= Context.Author;

        if (await db.Owners
                .Include(x => x.Cars)!.ThenInclude(x => x.Media)
                .Include(x => x.Cars)!.ThenInclude(x => x.Metadata)
                .FirstOrDefaultAsync(x => x.UserId == member.Id) is not { } owner)
        {
            return Response("That user hasn't added any cars to their profile!").AsEphemeral();
        }

        car ??= owner.Cars?.FirstOrDefault(x => x.Id == owner.DefaultCarId) ?? owner.Cars?.FirstOrDefault();
        
        if (car is null) //...shouldn't happen unless you remove all cars?
            return Response("That user hasn't added any cars to their profile!").AsEphemeral();

        var message = car.ToMessage<LocalInteractionMessageResponse>(member);
        return Response(message);
    }

    [SlashCommand("add")]
    [Description("Adds a new car to your profile.")]
    public async Task<IResult> Add(
        [Description("The model year of the car.")]
            int year,
        [Description("The make of the car.")]
        [Range(1, 100)]
            string make,
        [Description("The model of the car.")]
        [Range(1, 100)]
            string model,
        [Description("The color of the car.")]
        [Range(1, 100)]
            string color,
        [Name("media")]
        [Description("A single image or video of the car. (More media can be added later.)")]
        [Media, NonNitroAttachment]
            IAttachment? mediaAttachment = null)
    {
        await Deferral();

        var owner = await db.Owners.Include(x => x.Cars)!.ThenInclude(x => x.Media).FirstOrDefaultAsync(x => x.UserId == Context.AuthorId);

        if (owner is null)
        {
            owner = new CarOwner { UserId = Context.AuthorId, Cars = [] };
            db.Owners.Add(owner);
        }

        var car = new Car
        {
            Year = year,
            Make = make,
            Model = model,
            Color = color,
            Owner = owner,
            Media = [],
            Metadata = []
        };

        LocalAttachment? overrideAttachment = null;
        if (mediaAttachment is not null)
        {
            var media = await attachments.GetAttachmentAsync(mediaAttachment);
            var newMedia = new CarMedia();
            overrideAttachment = LocalAttachment.Bytes(media.Stream.ToArray(), media.FileName);
            await newMedia.UploadAsync(media);
            car.Media.Add(newMedia);
        }
        
        owner.Cars!.Add(car);
        if (owner.Cars.Count == 1) // first car
            owner.DefaultCarId = car.Id;
        
        await db.SaveChangesAsync();

        var response = car.ToMessage<LocalInteractionMessageResponse>(Context.Author);
        return Response(response);
    }

    [AutoComplete("add")]
    public async Task AutoCompleteMakesAndModels(AutoComplete<string> make, AutoComplete<string> model, AutoComplete<string> color)
    {
        if (make.IsFocused)
        {
            var makes = await db.Cars.Select(x => x.Make).Distinct().ToListAsync();
            make.AutoComplete(Context, makes);
            return;
        }

        if (model.IsFocused && make.Argument.TryGetValue(out var makeValue))
        {
            var models = await db.Cars.Where(x => x.Make == makeValue).Select(x => x.Model).Distinct().ToListAsync();
            model.AutoComplete(Context, models);
            return;
        }
        
        if (color.IsFocused && make.Argument.TryGetValue(out makeValue) && model.Argument.TryGetValue(out var modelValue))
        {
            var colors = await db.Cars.Where(x => x.Make == makeValue && x.Model == modelValue).Select(x => x.Color).Distinct().ToListAsync();
            color.AutoComplete(Context, colors);
        }
    }

    [AutoComplete("info")]
    public async Task AutoCompleteCars(AutoComplete<string> car)
    {
        if (!car.IsFocused)
            return;
        
        var interaction = (IAutoCompleteInteraction)Context.Interaction;
        var ownerId = interaction.Options.TryGetValue("member", out var raw) ? (Snowflake) raw.Value! : Context.AuthorId;
        var cars = await db.Cars.Where(x => x.OwnerId == ownerId).ToListAsync();
        car.AutoComplete(Context, cars);
    }

    [SlashGroup("metadata")]
    public sealed class CarMetadataModule(MiataDbContext db) : DiscordApplicationGuildModuleBase
    {
        [SlashCommand("set")]
        [Description("Sets or updates custom metadata about your car, such as aftermarket parts.")]
        public async Task<IResult> Set(
            [Description("The car you are setting metadata for.")]
            [RequireCarOwner]
                Car car,
            [Description("The metadata name (example: the type of aftermarket part).")]
            [Range(1, 25)]
                string name,
            [Description("The metadata value (example: the brand and/or model of aftermarket part).")]
            [Range(1, 100)]
                string value)
        {
            if (await db.Metadata.FirstOrDefaultAsync(x => x.CarId == car.Id && x.Name == name) is { } metadata)
            {
                metadata.Value = value;
            }
            else
            {
                db.Metadata.Add(new CarMetadata { CarId = car.Id, Name = name, Value = value });
            }

            await db.SaveChangesAsync();
            return Response("Metadata updated.").AsEphemeral();
        }

        [SlashCommand("remove")]
        [Description("Remove custom metadata from a car.")]
        public async Task<IResult> Remove(
            [Description("The car you are removing metadata from.")] 
            [RequireCarOwner]
                Car car,
            [Description("The metadata name (example: the type of aftermarket part).")] 
            [Range(1, 25)]
                string name)
        {
            await db.Metadata.Where(x => x.CarId == car.Id && x.Name == name).ExecuteDeleteAsync();
            return Response("Metadata removed.").AsEphemeral();
        }

        [AutoComplete("set")]
        [AutoComplete("remove")]
        public async Task AutoCompleteMetadataAsync(AutoComplete<string> car, AutoComplete<string> name)
        {
            if (car.IsFocused)
            {
                var cars = await db.Cars.Where(x => x.OwnerId == Context.AuthorId).ToListAsync();
                car.AutoComplete(Context, cars);
            }
            
            if (!name.IsFocused)
                return;
            
            var interaction = (IAutoCompleteInteraction)Context.Interaction;
            var rawId = interaction.Options.TryGetValue("car", out var raw) ? raw.ToString() : string.Empty;
            if (!Guid.TryParse(rawId, out var carId) ||
                await db.Cars.Include(x => x.Metadata).Where(x => x.OwnerId == Context.AuthorId && x.Id == carId).FirstOrDefaultAsync() is not { } dbCar)
            {
                return;
            }
            
            name.AutoComplete(Context, dbCar.Metadata!);
        }
    }

    [SlashGroup("media")]
    public sealed class CarMediaModule(MiataDbContext db, AttachmentService attachments, AmazonS3Client s3) : DiscordApplicationGuildModuleBase
    {
        public const int MAX_CAR_MEDIA = 4;
        
        [SlashCommand("add")]
        [Description("Attaches a new image or video to one of your cars (maximum 4).")]
        public async Task<IResult> Add(
            [Description("The car to add the media to.")]
            [RequireCarOwner]
                Car car,
            [Name("media")]
            [Description("An image or video of the car.")]
            [Media, NonNitroAttachment]
                IAttachment mediaAttachment)
        {
            var allMedia = await db.Media.Where(x => x.CarId == car.Id).ToListAsync();

            if (allMedia.Count >= MAX_CAR_MEDIA)
                return Response($"You cannot add more than {MAX_CAR_MEDIA} images or videos for this car!").AsEphemeral();

            await Deferral(true);
            
            var media = await attachments.GetAttachmentAsync(mediaAttachment);
            var newMedia = new CarMedia { CarId = car.Id };
            await newMedia.UploadAsync(media);
            db.Media.Add(newMedia);

            await db.SaveChangesAsync();
            return Response("New media added.").AsEphemeral();
        }
        
        [SlashCommand("clear")]
        [Description("Clear all media from your car. THIS CANNOT BE UNDONE.")]
        public async Task<IResult> Clear(
            [Description("The car to clear media from.")] 
            [RequireCarOwner]
                Car car)
        {
            await Deferral(true);
            
            var allMedia = await db.Media.Where(x => x.CarId == car.Id).ToListAsync();
            
            foreach (var media in allMedia)
            {
                db.Media.Remove(media);
            }

            await db.SaveChangesAsync();

            // TODO: remove this hardcoded bucket ID!
            // var bucket = bot.CurrentUser.Id.ToString();
            const string bucket = "580500893863116805";
            
            var objects = allMedia.Select(x => new KeyVersion { Key = x.ObjectKey.ToString() }).ToList();
            await s3.DeleteObjectsAsync(new DeleteObjectsRequest
            {
                BucketName = bucket,
                Objects = objects
            });

            return Response("All media cleared for this car.").AsEphemeral();
        }

        [AutoComplete("add")]
        [AutoComplete("clear")]
        public async Task AutoCompleteCarsAsync(AutoComplete<string> car)
        {
            if (!car.IsFocused)
                return;
            
            var cars = await db.Cars.Where(x => x.OwnerId == Context.AuthorId).ToListAsync();
            car.AutoComplete(Context, cars);
        }
    }
}