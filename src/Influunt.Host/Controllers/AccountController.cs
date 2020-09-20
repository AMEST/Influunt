using Influunt.Feed;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

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

        [HttpGet("login/google")]
        public async Task<IActionResult> SignInGoogle()
        {
            var user = await _userService.GetCurrentUser();
            if(user == null || string.IsNullOrWhiteSpace(user.Email))
                return Challenge("Google");

            return RedirectPermanent("/");
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectPermanent("/");
        }
    }
}