using Disqord;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MiataBot;

public sealed class SnowflakeConverter() : ValueConverter<Snowflake, long>(x => (long) x.RawValue, x => new Snowflake((ulong) x));