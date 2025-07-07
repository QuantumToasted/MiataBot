namespace MiataBot;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class MediaAttribute() : RequireAttachmentExtensionsAttribute("png", "jpeg", "jpg", "webp", "mp4", "webm");