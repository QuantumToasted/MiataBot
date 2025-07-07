namespace MiataBot;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class NonNitroAttachmentAttribute() : MaximumAttachmentSizeAttribute(8.00, FileSizeMeasure.MB);