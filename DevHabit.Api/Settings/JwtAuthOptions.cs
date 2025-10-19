﻿using System.ComponentModel.DataAnnotations;

namespace DevHabit.Api.Settings
{
  public sealed class JwtAuthOptions
  {
    public required string Audience { get; init; }
    public required int ExpirationInMinutes { get; init; }
    public required string Issuer { get; init; }

    public required string Key { get; init; }
    public required int RefreshTokenExpirationInDays { get; init; }
  }
}
