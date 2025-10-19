using System.Net.Mime;
using DevHabit.Api.CustomMediaTypes;
using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Auth;
using DevHabit.Api.Dtos.Users;
using DevHabit.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevHabit.Api.Controllers
{
  [ApiController]
  [Route("auth")]
  [AllowAnonymous]
  [Produces(MediaTypeNames.Application.Json, ApplicationMediaTypes.DevHabitApi)]
  public class AuthController : ControllerBase
  {
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ApplicationIdentityDbContext _identityDbContext;
    private readonly TokenProvider _tokenProvider;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(
      ApplicationIdentityDbContext identityDbContext,
      UserManager<IdentityUser> userManager,
      ApplicationDbContext applicationDbContext,
      TokenProvider tokenProvider
    )
    {
      ArgumentNullException.ThrowIfNull(identityDbContext);
      ArgumentNullException.ThrowIfNull(userManager);
      ArgumentNullException.ThrowIfNull(applicationDbContext);
      ArgumentNullException.ThrowIfNull(tokenProvider);

      _identityDbContext = identityDbContext;
      _userManager = userManager;
      _applicationDbContext = applicationDbContext;
      _tokenProvider = tokenProvider;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AccessTokensDto>> Login(
      [FromBody] LoginUserDto loginUserDto,
      [FromServices] IValidator<LoginUserDto> validator
    )
    {
      ArgumentNullException.ThrowIfNull(validator);

      await validator
        .ValidateAndThrowAsync(loginUserDto, HttpContext.RequestAborted)
        .ConfigureAwait(false);

      var identityUser = await _userManager
        .FindByEmailAsync(loginUserDto.Email)
        .ConfigureAwait(false);

      if (identityUser is null)
      {
        return Unauthorized();
      }

      var passwordValid = await _userManager
        .CheckPasswordAsync(identityUser, loginUserDto.Password)
        .ConfigureAwait(false);

      if (!passwordValid)
      {
        return Unauthorized();
      }

      var tokenRequestDto = new TokenRequestDto
      {
        UserId = identityUser.Id,
        Email = identityUser.Email!,
      };
      var accessTokensDto = _tokenProvider.CreateToken(tokenRequestDto);

      return Ok(accessTokensDto);
    }

    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AccessTokensDto>> Register(
      [FromBody] RegisterUserDto registerUserDto,
      [FromServices] IValidator<RegisterUserDto> validator
    )
    {
      ArgumentNullException.ThrowIfNull(validator);

      await validator
        .ValidateAndThrowAsync(registerUserDto, HttpContext.RequestAborted)
        .ConfigureAwait(false);

      var transaction = await _identityDbContext
        .Database.BeginTransactionAsync(HttpContext.RequestAborted)
        .ConfigureAwait(false);

      await using (transaction)
      {
        _applicationDbContext.Database.SetDbConnection(
          _identityDbContext.Database.GetDbConnection()
        );

        await _applicationDbContext
          .Database.UseTransactionAsync(transaction.GetDbTransaction(), HttpContext.RequestAborted)
          .ConfigureAwait(false);

        var identityUser = new IdentityUser
        {
          Email = registerUserDto.Email,
          UserName = registerUserDto.Email,
        };

        var identityResult = await _userManager
          .CreateAsync(identityUser, registerUserDto.Password)
          .ConfigureAwait(false);

        if (!identityResult.Succeeded)
        {
          var extensions = new Dictionary<string, object?>
          {
            { "errors", identityResult.Errors.ToDictionary(e => e.Code, e => e.Description) },
          };

          return Problem(
            detail: "User registration failed.",
            statusCode: StatusCodes.Status400BadRequest,
            extensions: extensions
          );
        }

        var user = registerUserDto.ToEntity();
        user.IdentityId = identityUser.Id;

        await _applicationDbContext
          .Users.AddAsync(user, HttpContext.RequestAborted)
          .ConfigureAwait(false);

        await _applicationDbContext
          .SaveChangesAsync(HttpContext.RequestAborted)
          .ConfigureAwait(false);

        await transaction.CommitAsync(HttpContext.RequestAborted).ConfigureAwait(false);

        var accessTokenDto = _tokenProvider.CreateToken(
          new TokenRequestDto { Email = identityUser.Email, UserId = identityUser.Id }
        );

        return Ok(accessTokenDto);
      }
    }
  }
}
