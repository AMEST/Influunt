using System.Collections.Generic;
using System.Security.Claims;
using Influunt.Feed;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Influunt.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("current")]
        public async Task<IActionResult> CurrentUser()
        {
            var user = await _userService.GetCurrentUser();

            return new JsonResult(user);
        }

        [HttpGet("login")]
        [Authorize]
        public IActionResult SignIn()
        {
            return Redirect("/");
        }

        [HttpGet("login/google")]
        public async Task<IActionResult> SignInGoogle()
        {
            var user = await _userService.GetCurrentUser();

            if (user != null && !string.IsNullOrWhiteSpace(user.Email)) 
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
        /// <returns></returns>
        [HttpGet("login/guest")]
        public async Task<IActionResult> SignInAsGuest()
        {
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name, "Guest"),
                new Claim(ClaimTypes.Email, "guest@local"),
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, "GuestScheme");

            var authProperties = new AuthenticationProperties();
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            return Redirect("/");

        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}