using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Disqord;
using Disqord.Bot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Qommon;

namespace MiataBot;

public static class DbModelExtensions
{
    private const string DOWNLOAD_URL_BASE = "https://f005.backblazeb2.com/file"; // TODO: is this reliable?
    
    public static IServiceProvider Services { get; set; } = null!;

    public static async Task<bool> UploadAsync(this CarMedia media, LocalAttachment attachment)
    {
        var s3 = Services.GetRequiredService<AmazonS3Client>();
        var bot = Services.GetRequiredService<DiscordBotBase>();

        // TODO: remove this hardcoded bucket ID!
        // var bucket = bot.CurrentUser.Id.ToString();
        const string bucket = "580500893863116805";
        var key = media.ObjectKey.ToString();

        try
        {
            var fileName = Path.GetFileName(attachment.FileName.Value);
            await s3.PutObjectAsync(new PutObjectRequest
            {
                BucketName = bucket,
                Key = key,
                InputStream = attachment.Stream.Value,
                ContentType = MimeTypes.GetMimeType(fileName)
            });
            
            return true;
        }
        catch (Exception ex)
        {
            bot.Logger.LogError(ex, "Failed to PUT object to B2.");
            return false;
        }
    }

    public static async Task<bool> DeleteAsync(this CarMedia media)
    {
        var s3 = Services.GetRequiredService<AmazonS3Client>();
        var bot = Services.GetRequiredService<DiscordBotBase>();

        // TODO: remove this hardcoded bucket ID!
        // var bucket = bot.CurrentUser.Id.ToString();
        const string bucket = "580500893863116805";
        var key = media.ObjectKey.ToString();
        
        try
        {
            await s3.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = bucket, 
                Key = key
            });
            
            return true;
        }
        catch (Exception ex)
        {
            bot.Logger.LogError(ex, "Failed to DELETE object from B2.");
            return false;
        }
    }

    public static string? GetUrl(this CarMedia media)
    {
        var bot = Services.GetRequiredService<DiscordBotBase>();

        // TODO: remove this hardcoded bucket ID!
        // var bucket = bot.CurrentUser.Id.ToString();
        const string bucket = "580500893863116805";
        var key = media.ObjectKey.ToString();
        
        try
        {
            return $"{DOWNLOAD_URL_BASE}/{bucket}/{key}";
        }
        catch (Exception ex)
        {
            bot.Logger.LogError(ex, "Failed to get car media URL.");
            return null;
        }
    }

    public static TMessage ToMessage<TMessage>(this Car car, IMember owner)
        where TMessage : LocalMessageBase, new()
    {
        Guard.IsNotNull(car.Owner);
        Guard.IsNotNull(car.Media);
        Guard.IsNotNull(car.Metadata);
        
        var message = new TMessage();

        var headerContentBuilder = new StringBuilder($"## {owner.GetDisplayName()}'s {car.Year} {car.Make} {car.Model}");

        if (!string.IsNullOrWhiteSpace(car.PetName))
            headerContentBuilder.Append($" — \"{car.PetName}\"");
        
        headerContentBuilder.AppendNewline()
            .AppendNewline($"{Markdown.Bold("Color:")} {car.Color}")
            .AppendJoin("\n", car.Metadata.Select(x => x.Format()));
        
        var headerSection = LocalComponent.Section(
            LocalComponent.Thumbnail(owner.GetGuildAvatarUrl()), LocalComponent.TextDisplay(headerContentBuilder.ToString()));
        
        var container = LocalComponent.Container(owner.GetHighestRole(r => r.Color.HasValue)?.Color ?? Color.Random);

        container.AddComponent(headerSection)
            .AddComponent(LocalComponent.Separator());

        if (!string.IsNullOrWhiteSpace(car.Blurb))
        {
            container.AddComponent(LocalComponent.TextDisplay(Markdown.Italics($"\"{car.Blurb}\"")))
                .AddComponent(LocalComponent.Separator());
        }

        if (car.Media.Count > 0)
        {
            List<LocalMediaGalleryItem> mediaItems = [];

            foreach (var carMedia in car.Media)
            {
                if (carMedia.GetUrl() is { } mediaUrl)
                    mediaItems.Add(new LocalMediaGalleryItem().WithMedia(mediaUrl));
            }

            container.AddComponent(LocalComponent.MediaGallery(mediaItems));
        }

        message.AddComponent(container);

        return message;
    }

    public static string Format(this CarMetadata metadata)
        => $"{Markdown.Bold($"{metadata.Name}:")} {metadata.Value}";
}