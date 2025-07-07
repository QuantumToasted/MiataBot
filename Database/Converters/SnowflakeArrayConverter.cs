﻿using Disqord;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.ValueConversion;

namespace MiataBot;

public sealed class SnowflakeArrayConverter()
    : NpgsqlArrayConverter<Snowflake[], Snowflake[], long[]>(
        new ValueConverter<Snowflake, long>(static x => (long)x.RawValue, static x => new Snowflake((ulong)x)));