using System.Collections.Generic;
using System.Security.Claims;
using Influunt.Feed;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Influunt.Host.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Influunt.Host.Controllers;

/// <summary>
/// Account api
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;

    /// <inheritdoc />
    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Returns current user profile or null
    /// </summary>
    /// <response code="200">User profile</response>
    [HttpGet("current")]
    [ProducesResponseType(typeof(UserViewModel),200)]
    public async Task<IActionResult> CurrentUser()
    {
        var user = await _userService.GetCurrentUser();

        return new JsonResult(user.ToModel());
    }

    /// <summary>
    /// SignIn via default challenge
    /// </summary>
    /// <response code="302">Redirect to application home after signin</response>
    [HttpGet("login")]
    [Authorize]
    public IActionResult SignIn()
    {
        return Redirect("/");
    }

    /// <summary>
    /// SignIn via Google challenge
    /// </summary>
    /// <response code="302">Redirect to application home after signin</response>
    [HttpGet("login/google")]
    public async Task<IActionResult> SignInGoogle()
    {
        var user = await _userService.GetCurrentUser();

        if (!string.IsNullOrWhiteSpace(user?.Email)) 
            return Redirect("/");

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true
        };
        return Challenge(authProperties,"Google");
    }

    /// <summary>
    /// Sing In as guest for try service
    /// </summary>
    /// <response code="302">Redirect to application home after signin</response>
    [HttpGet("login/guest")]
    public async Task<IActionResult> SignInAsGuest()
    {
        var claims = new List<Claim>{
            new(ClaimTypes.Name, "Guest"),
            new(ClaimTypes.Email, "guest@local"),
        };
        var claimsIdentity = new ClaimsIdentity(
            claims, "GuestScheme");

        var authProperties = new AuthenticationProperties();
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), authProperties);

        return Redirect("/");

    }

    /// <summary>
    /// SignOut from service
    /// </summary>
    /// <response code="302">Redirect to application home after signout</response>
    [HttpGet("[action]")]
    public new async Task<IActionResult> SignOut()
    {
        await HttpContext.SignOutAsync();
        return Redirect("/");
    }
}