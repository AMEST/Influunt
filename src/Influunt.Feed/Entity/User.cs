using System;
using System.Linq;
using System.Security.Claims;
using Skidbladnir.Repository.Abstractions;

namespace Influunt.Feed.Entity;

public class User : IHasId<string>
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string AuthProvider { get; set; }
    public string Id { get; set; }
    public DateTime LastActivity { get; set; }

    public bool IsNullable()
    {
        if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(Name))
            return true;
        return false;
    }

    public static User FromIdentity(ClaimsPrincipal principal)
    {
        var user = new User
        {
            Email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
            AuthProvider = principal.Identity.AuthenticationType,
            Name = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
        };

        return user.IsNullable() ? null : user;
    }
}