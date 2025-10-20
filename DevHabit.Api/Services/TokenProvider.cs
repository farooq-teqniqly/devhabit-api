using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DevHabit.Api.Dtos.Auth;
using DevHabit.Api.Settings;
using FluentValidation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DevHabit.Api.Services
{
  public sealed class TokenProvider
  {
    private readonly JwtAuthOptions _jwtAuthOptions;
    private readonly IValidator<TokenRequestDto> _tokenRequestDtoValidator;

    public TokenProvider(
      IOptions<JwtAuthOptions> options,
      IValidator<JwtAuthOptions> jwtAuthOptionsValidator,
      IValidator<TokenRequestDto> tokenRequestDtoValidator
    )
    {
      ArgumentNullException.ThrowIfNull(options);
      ArgumentNullException.ThrowIfNull(jwtAuthOptionsValidator);
      ArgumentNullException.ThrowIfNull(tokenRequestDtoValidator);

      _jwtAuthOptions = options.Value;
      _tokenRequestDtoValidator = tokenRequestDtoValidator;
      jwtAuthOptionsValidator.ValidateAndThrow(_jwtAuthOptions);
    }

    public AccessTokensDto CreateToken(TokenRequestDto tokenRequest)
    {
      ArgumentNullException.ThrowIfNull(tokenRequest);

      _tokenRequestDtoValidator.ValidateAndThrow(tokenRequest);

      return new AccessTokensDto(GenerateAccessToken(tokenRequest), GenerateRefreshToken());
    }

    private static string GenerateRefreshToken() =>
      Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    private string GenerateAccessToken(TokenRequestDto tokenRequest)
    {
      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.Key));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

      var claims = new List<Claim>
      {
        new(JwtRegisteredClaimNames.Sub, tokenRequest.UserId),
        new(JwtRegisteredClaimNames.Email, tokenRequest.Email),
      };

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(_jwtAuthOptions.ExpirationInMinutes),
        SigningCredentials = credentials,
        Issuer = _jwtAuthOptions.Issuer,
        Audience = _jwtAuthOptions.Audience,
      };

      var handler = new JsonWebTokenHandler();
      var accessToken = handler.CreateToken(tokenDescriptor);

      return accessToken;
    }
  }
}
