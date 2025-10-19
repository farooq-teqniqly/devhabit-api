using System.Net.Mime;
using DevHabit.Api.CustomMediaTypes;
using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Auth;
using DevHabit.Api.Dtos.Users;
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
    private readonly ApplicationIdentityDbContext _identityDbContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _applicationDbContext;

    public AuthController(
      ApplicationIdentityDbContext identityDbContext,
      UserManager<IdentityUser> userManager,
      ApplicationDbContext applicationDbContext
    )
    {
      ArgumentNullException.ThrowIfNull(identityDbContext);
      ArgumentNullException.ThrowIfNull(userManager);
      ArgumentNullException.ThrowIfNull(applicationDbContext);

      _identityDbContext = identityDbContext;
      _userManager = userManager;
      _applicationDbContext = applicationDbContext;
    }

    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
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

        var identityResult = await _userManager.CreateAsync(identityUser).ConfigureAwait(false);

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

        return Ok(user.Id);
      }
    }
  }
}
